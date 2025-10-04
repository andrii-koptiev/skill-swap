using Microsoft.Extensions.DependencyInjection;
using SkillSwap.Application.Interfaces;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Infrastructure.Repositories;

namespace SkillSwap.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering repository and Unit of Work services
/// </summary>
public static class RepositoryServiceExtensions
{
    /// <summary>
    /// Registers all repository and Unit of Work services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection for method chaining</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Register Unit of Work - requires ILoggerFactory
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register specific repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();
        services.AddScoped<ISkillCategoryRepository, SkillCategoryRepository>();
        services.AddScoped<IUserSkillRepository, UserSkillRepository>();
        services.AddScoped<IUserAvailabilityRepository, UserAvailabilityRepository>();
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        // Register generic repository for other entities
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
