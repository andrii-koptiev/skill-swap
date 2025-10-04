using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// User Preferences repository interface
/// </summary>
public interface IUserPreferencesRepository : IRepository<UserPreferences>
{
    /// <summary>
    /// Get preferences by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User preferences if found, null otherwise</returns>
    Task<UserPreferences?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
