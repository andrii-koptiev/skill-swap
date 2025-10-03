using System.Net;
using System.Text.Json;
using SkillSwap.Api.Extensions;
using SkillSwap.Domain.Common;

namespace SkillSwap.Api.Middleware;

/// <summary>
/// Global exception handling middleware that provides centralized error handling,
/// structured logging, and consistent API error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment
    )
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException ex) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected or request was cancelled - log as info, not error
            _logger.LogInformation(
                ex,
                "Request cancelled for {RequestPath} {RequestMethod}. TraceId: {TraceId}",
                LoggingExtensions.SanitizeRequestPathForLog(context.Request.Path),
                LoggingExtensions.SanitizeRequestPathForLog(context.Request.Method),
                context.TraceIdentifier
            );
            // Don't handle cancellation - let it propagate with context
            throw new OperationCanceledException(
                $"Request {context.Request.Method} {context.Request.Path} was cancelled. TraceId: {context.TraceIdentifier}",
                ex
            );
        }
        catch (Exception ex)
            when (ex
                    is not StackOverflowException
                        and not OutOfMemoryException
                        and not ThreadAbortException
            )
        {
            // Sanitize Path and Method to prevent log forging
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing request {RequestPath} {RequestMethod}",
                LoggingExtensions.SanitizeRequestPathForLog(context.Request.Path),
                LoggingExtensions.SanitizeRequestPathForLog(context.Request.Method)
            );

            await HandleExceptionAsync(context, ex);
        }
        // Critical exceptions (StackOverflowException, OutOfMemoryException, ThreadAbortException)
        // are intentionally not caught and will propagate up
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, error) = GetErrorResponse(exception);
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Code = error.Code,
            Message = error.Message,
            Details = _environment.IsDevelopment() ? error.Details : null,
            Errors = error.Errors,
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow,
        };

        var jsonResponse = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        await context.Response.WriteAsync(jsonResponse);
    }

    private (HttpStatusCode statusCode, ErrorDetails error) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            DomainException domainEx => HandleDomainException(domainEx),
            FluentValidation.ValidationException fluentValidationEx =>
                HandleFluentValidationException(fluentValidationEx),
            SkillSwap.Domain.Common.ValidationException domainValidationEx =>
                HandleDomainValidationException(domainValidationEx),
            UnauthorizedAccessException => HandleUnauthorizedAccessException(),
            ArgumentNullException argEx => HandleArgumentException(argEx),
            ArgumentException argEx => HandleArgumentException(argEx),
            KeyNotFoundException => HandleKeyNotFoundException(),
            InvalidOperationException opEx => HandleInvalidOperationException(opEx),
            _ => HandleGenericException(exception),
        };
    }

    private (HttpStatusCode, ErrorDetails) HandleDomainException(DomainException domainEx)
    {
        return (
            HttpStatusCode.BadRequest,
            new ErrorDetails
            {
                Code = "DOMAIN_RULE_VIOLATION",
                Message = domainEx.Message,
                Details = _environment.IsDevelopment() ? domainEx.ToString() : null,
            }
        );
    }

    private (HttpStatusCode, ErrorDetails) HandleFluentValidationException(
        FluentValidation.ValidationException fluentValidationEx
    )
    {
        // Normalize FluentValidation errors to consistent structure
        var normalizedErrors = NormalizeValidationErrors(fluentValidationEx.Errors);

        return (
            HttpStatusCode.BadRequest,
            new ErrorDetails
            {
                Code = "VALIDATION_FAILED",
                Message = "One or more validation failures occurred.",
                Details = _environment.IsDevelopment() ? fluentValidationEx.ToString() : null,
                Errors = normalizedErrors,
            }
        );
    }

    private (HttpStatusCode, ErrorDetails) HandleDomainValidationException(
        ValidationException domainValidationEx
    )
    {
        // Use existing domain validation errors if available, otherwise normalize from exception
        var normalizedErrors =
            domainValidationEx.Errors.Count > 0
                ? domainValidationEx.Errors
                : NormalizeValidationErrors(domainValidationEx.Message);

        return (
            HttpStatusCode.BadRequest,
            new ErrorDetails
            {
                Code = "VALIDATION_FAILED",
                Message =
                    normalizedErrors.Count > 0
                        ? "One or more validation failures occurred."
                        : "Validation failed",
                Details = _environment.IsDevelopment() ? domainValidationEx.ToString() : null,
                Errors = normalizedErrors.Count > 0 ? normalizedErrors : null,
            }
        );
    }

    /// <summary>
    /// Normalizes FluentValidation errors into a consistent dictionary structure.
    /// </summary>
    /// <param name="validationFailures">FluentValidation error collection</param>
    /// <returns>Dictionary with property names as keys and error messages as values</returns>
    private static Dictionary<string, string[]> NormalizeValidationErrors(
        IEnumerable<FluentValidation.Results.ValidationFailure> validationFailures
    )
    {
        return validationFailures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray()
            );
    }

    /// <summary>
    /// Normalizes a single validation message into the dictionary structure.
    /// </summary>
    /// <param name="validationMessage">Single validation error message</param>
    /// <returns>Dictionary with general error key and message</returns>
    private static Dictionary<string, string[]> NormalizeValidationErrors(string validationMessage)
    {
        return new Dictionary<string, string[]> { ["General"] = [validationMessage] };
    }

    private static (HttpStatusCode, ErrorDetails) HandleUnauthorizedAccessException()
    {
        return (
            HttpStatusCode.Unauthorized,
            new ErrorDetails
            {
                Code = "UNAUTHORIZED_ACCESS",
                Message = "Unauthorized access",
                Details = null, // Never expose auth details
            }
        );
    }

    private (HttpStatusCode, ErrorDetails) HandleArgumentException(Exception argEx)
    {
        return (
            HttpStatusCode.BadRequest,
            new ErrorDetails
            {
                Code = "INVALID_PARAMETERS",
                Message = "Invalid request parameters",
                Details = _environment.IsDevelopment() ? argEx.Message : null,
            }
        );
    }

    private static (HttpStatusCode, ErrorDetails) HandleKeyNotFoundException()
    {
        return (
            HttpStatusCode.NotFound,
            new ErrorDetails
            {
                Code = "RESOURCE_NOT_FOUND",
                Message = "Resource not found",
                Details = null,
            }
        );
    }

    private (HttpStatusCode, ErrorDetails) HandleInvalidOperationException(
        InvalidOperationException opEx
    )
    {
        return (
            HttpStatusCode.BadRequest,
            new ErrorDetails
            {
                Code = "INVALID_OPERATION",
                Message = "Invalid operation",
                Details = _environment.IsDevelopment() ? opEx.Message : null,
            }
        );
    }

    private (HttpStatusCode, ErrorDetails) HandleGenericException(Exception exception)
    {
        return (
            HttpStatusCode.InternalServerError,
            new ErrorDetails
            {
                Code = "INTERNAL_SERVER_ERROR",
                Message = "An internal server error occurred",
                Details = _environment.IsDevelopment() ? exception.ToString() : null,
            }
        );
    }

    /// <summary>
    /// Error details for API responses.
    /// </summary>
    private sealed record ErrorDetails
    {
        /// <summary>
        /// Machine-readable error code for client-side handling and localization.
        /// </summary>
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Additional error details (only in development environment).
        /// </summary>
        public string? Details { get; init; }

        /// <summary>
        /// Field-level validation errors (for validation failures).
        /// </summary>
        public Dictionary<string, string[]>? Errors { get; init; }
    }

    /// <summary>
    /// Standard API error response format.
    /// </summary>
    private sealed record ErrorResponse
    {
        /// <summary>
        /// Machine-readable error code for client-side handling and localization.
        /// </summary>
        public string Code { get; init; } = string.Empty;

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Additional error details (only in development environment).
        /// </summary>
        public string? Details { get; init; }

        /// <summary>
        /// Field-level validation errors (for validation failures).
        /// </summary>
        public Dictionary<string, string[]>? Errors { get; init; }

        /// <summary>
        /// Request trace identifier for debugging.
        /// </summary>
        public string TraceId { get; init; } = string.Empty;

        /// <summary>
        /// Error occurrence timestamp in UTC.
        /// </summary>
        public DateTime Timestamp { get; init; }
    }
}
