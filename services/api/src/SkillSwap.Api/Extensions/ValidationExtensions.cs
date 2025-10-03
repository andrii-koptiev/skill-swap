using FluentValidation;

namespace SkillSwap.Api.Extensions;

/// <summary>
/// Extension methods for configuring FluentValidation.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Configures FluentValidation services.
    /// </summary>
    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        // Register all validators from the API assembly
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
