using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// User Skill repository interface with user skill-specific operations
/// </summary>
public interface IUserSkillRepository : IRepository<UserSkill>
{
    /// <summary>
    /// Get user skills by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User skills</returns>
    Task<IEnumerable<UserSkill>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get users by skill ID
    /// </summary>
    /// <param name="skillId">Skill ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User skills for the specified skill</returns>
    Task<IEnumerable<UserSkill>> GetBySkillIdAsync(
        Guid skillId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get user skill by user and skill IDs
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="skillId">Skill ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User skill if found, null otherwise</returns>
    Task<UserSkill?> GetByUserAndSkillAsync(
        Guid userId,
        Guid skillId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get user skills by proficiency level
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="proficiency">Proficiency level (1-5)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User skills with the specified proficiency</returns>
    Task<IEnumerable<UserSkill>> GetByProficiencyAsync(
        Guid userId,
        int proficiency,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get skills user can teach
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Skills user can teach</returns>
    Task<IEnumerable<UserSkill>> GetTeachableSkillsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get skills user wants to learn
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Skills user wants to learn</returns>
    Task<IEnumerable<UserSkill>> GetLearningSkillsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if user has skill
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="skillId">Skill ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user has the skill, false otherwise</returns>
    Task<bool> UserHasSkillAsync(
        Guid userId,
        Guid skillId,
        CancellationToken cancellationToken = default
    );
}
