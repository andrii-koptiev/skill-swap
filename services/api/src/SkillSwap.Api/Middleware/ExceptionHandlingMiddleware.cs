using System.Net;
using System.Text.Json;
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
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred while processing request {RequestPath} {RequestMethod}",
                context.Request.Path,
                context.Request.Method
            );

            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, error) = GetErrorResponse(exception);
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            Message = error.Message,
            Details = _environment.IsDevelopment() ? error.Details : null,
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
            DomainException domainEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails
                {
                    Message = domainEx.Message,
                    Details = _environment.IsDevelopment() ? domainEx.ToString() : null,
                }
            ),
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails
                {
                    Message = "Validation failed",
                    Details = _environment.IsDevelopment() ? validationEx.ToString() : null,
                }
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                new ErrorDetails
                {
                    Message = "Unauthorized access",
                    Details = null, // Never expose auth details
                }
            ),
            ArgumentNullException argEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails
                {
                    Message = "Invalid request parameters",
                    Details = _environment.IsDevelopment() ? argEx.Message : null,
                }
            ),
            ArgumentException argEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails
                {
                    Message = "Invalid request parameters",
                    Details = _environment.IsDevelopment() ? argEx.Message : null,
                }
            ),
            KeyNotFoundException => (
                HttpStatusCode.NotFound,
                new ErrorDetails { Message = "Resource not found", Details = null }
            ),
            InvalidOperationException opEx => (
                HttpStatusCode.BadRequest,
                new ErrorDetails
                {
                    Message = "Invalid operation",
                    Details = _environment.IsDevelopment() ? opEx.Message : null,
                }
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                new ErrorDetails
                {
                    Message = "An internal server error occurred",
                    Details = _environment.IsDevelopment() ? exception.ToString() : null,
                }
            ),
        };
    }

    private sealed record ErrorDetails
    {
        public string Message { get; init; } = string.Empty;
        public string? Details { get; init; }
    }

    private sealed record ErrorResponse
    {
        public string Message { get; init; } = string.Empty;
        public string? Details { get; init; }
        public string TraceId { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
    }
}
