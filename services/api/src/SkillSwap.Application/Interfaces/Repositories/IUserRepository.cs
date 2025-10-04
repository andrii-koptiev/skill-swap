using SkillSwap.Domain.Entities;
using SkillSwap.Domain.ValueObjects;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// User repository interface with user-specific operations
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user by username
    /// </summary>
    /// <param name="username">Username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User if found, null otherwise</returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if email exists
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if username exists
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if username exists, false otherwise</returns>
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users with specific skills
    /// </summary>
    /// <param name="skillIds">Skill IDs to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users with the specified skills</returns>
    Task<IEnumerable<User>> GetUsersBySkillsAsync(
        IEnumerable<Guid> skillIds,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get user with their skills included
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User with skills if found, null otherwise</returns>
    Task<User?> GetWithSkillsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user with their availability included
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User with availability if found, null otherwise</returns>
    Task<User?> GetWithAvailabilityAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );
}
