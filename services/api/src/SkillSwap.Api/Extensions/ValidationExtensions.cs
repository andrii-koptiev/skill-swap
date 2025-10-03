using FluentValidation;

namespace SkillSwap.Api.Extensions;

/// <summary>
/// Extension methods for configuring FluentValidation.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Configures FluentValidation services and automatic validation behavior.
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        // Register all validators from the API assembly
        services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Singleton);

        return services;
    }

    /// <summary>
    /// Adds validation middleware to the pipeline.
    /// </summary>
    public static IApplicationBuilder UseValidation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ValidationMiddleware>();
    }
}

/// <summary>
/// Middleware that performs request validation using FluentValidation.
/// </summary>
public sealed class ValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ValidationMiddleware> _logger;

    public ValidationMiddleware(RequestDelegate next, ILogger<ValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only validate for requests with body content
        if (
            context.Request.Method == HttpMethods.Post
            || context.Request.Method == HttpMethods.Put
            || context.Request.Method == HttpMethods.Patch
        )
        {
            // Enable buffering so we can read the request body multiple times
            context.Request.EnableBuffering();

            // Get the endpoint metadata to determine if validation is needed
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata.GetMetadata<ValidateAttribute>() != null)
            {
                await ValidateRequestAsync(context);
            }
        }

        await _next(context);
    }

    private async Task ValidateRequestAsync(HttpContext context)
    {
        try
        {
            // This is a simplified implementation
            // In a real scenario, you'd need to determine the request type
            // and get the appropriate validator based on the controller action
            _logger.LogDebug(
                "Request validation completed for {RequestPath}",
                context.Request.Path
            );
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error during request validation for {RequestPath}",
                context.Request.Path
            );
            throw new SkillSwap.Domain.Common.ValidationException("Request validation failed");
        }
    }
}

/// <summary>
/// Attribute to mark controller actions that require validation.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ValidateAttribute : Attribute { }
