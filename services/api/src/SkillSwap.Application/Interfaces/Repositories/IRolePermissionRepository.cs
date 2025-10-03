using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// Role Permission repository interface
/// </summary>
public interface IRolePermissionRepository : IRepository<RolePermission>
{
    /// <summary>
    /// Get permissions by role ID
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role permissions</returns>
    Task<IEnumerable<RolePermission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get roles by permission
    /// </summary>
    /// <param name="permission">Permission name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role permissions for the specified permission</returns>
    Task<IEnumerable<RolePermission>> GetByPermissionAsync(
        string permission,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get role permission by role and permission
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permission">Permission name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role permission if found, null otherwise</returns>
    Task<RolePermission?> GetByRoleAndPermissionAsync(
        Guid roleId,
        string permission,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if role has permission
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="permission">Permission name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if role has the permission, false otherwise</returns>
    Task<bool> RoleHasPermissionAsync(
        Guid roleId,
        string permission,
        CancellationToken cancellationToken = default
    );
}
