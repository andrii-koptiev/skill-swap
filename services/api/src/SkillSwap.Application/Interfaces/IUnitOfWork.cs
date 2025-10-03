using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Common;

namespace SkillSwap.Application.Interfaces;

/// <summary>
/// Unit of Work interface for managing transactions and repositories
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// User repository
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Skill repository
    /// </summary>
    ISkillRepository Skills { get; }

    /// <summary>
    /// Skill Category repository
    /// </summary>
    ISkillCategoryRepository SkillCategories { get; }

    /// <summary>
    /// User Skill repository
    /// </summary>
    IUserSkillRepository UserSkills { get; }

    /// <summary>
    /// User Availability repository
    /// </summary>
    IUserAvailabilityRepository UserAvailability { get; }

    /// <summary>
    /// User Preferences repository
    /// </summary>
    IUserPreferencesRepository UserPreferences { get; }

    /// <summary>
    /// Role repository
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// User Role repository
    /// </summary>
    IUserRoleRepository UserRoles { get; }

    /// <summary>
    /// Role Permission repository
    /// </summary>
    IRolePermissionRepository RolePermissions { get; }

    /// <summary>
    /// Save all changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of entities saved</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begin a database transaction
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Database transaction</returns>
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get a generic repository for any entity type
    /// </summary>
    /// <typeparam name="T">Entity type that inherits from BaseEntity</typeparam>
    /// <returns>Repository instance</returns>
    IRepository<T> Repository<T>()
        where T : BaseEntity;
}
