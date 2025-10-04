using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// Role repository interface
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// Get role by name
    /// </summary>
    /// <param name="name">Role name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role if found, null otherwise</returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get role with permissions
    /// </summary>
    /// <param name="roleId">Role ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Role with permissions if found, null otherwise</returns>
    Task<Role?> GetWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if role name exists
    /// </summary>
    /// <param name="name">Role name</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> NameExistsAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
}
