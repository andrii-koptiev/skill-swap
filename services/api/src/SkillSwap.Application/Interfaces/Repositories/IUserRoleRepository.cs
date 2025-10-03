using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// User Role repository interface
/// </summary>
public interface IUserRoleRepository : IRepository<UserRole>
{
    /// <summary>
    /// Get user roles by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User roles</returns>
    Task<IEnumerable<UserRole>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get users by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User roles for the specified role</returns>
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get user role by user and role IDs
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User role if found, null otherwise</returns>
    Task<UserRole?> GetByUserAndRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if user has role
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if user has role by name
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="roleName">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has the role, false otherwise</returns>
    Task<bool> UserHasRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default
    );
}
