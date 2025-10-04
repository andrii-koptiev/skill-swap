using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// Skill repository interface with skill-specific operations
/// </summary>
public interface ISkillRepository : IRepository<Skill>
{
    /// <summary>
    /// Get skill by name
    /// </summary>
    /// <param name="name">Skill name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Skill if found, null otherwise</returns>
    Task<Skill?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get skill by slug
    /// </summary>
    /// <param name="slug">Skill slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Skill if found, null otherwise</returns>
    Task<Skill?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get skills by category
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Skills in the specified category</returns>
    Task<IEnumerable<Skill>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get active skills only
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active skills</returns>
    Task<IEnumerable<Skill>> GetActiveSkillsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Search skills by name or description
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Matching skills</returns>
    Task<IEnumerable<Skill>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Check if skill name exists in category
    /// </summary>
    /// <param name="name">Skill name</param>
    /// <param name="categoryId">Category ID</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists in category, false otherwise</returns>
    Task<bool> NameExistsInCategoryAsync(
        string name,
        Guid categoryId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
}
