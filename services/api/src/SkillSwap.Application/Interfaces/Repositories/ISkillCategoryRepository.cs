using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// Skill Category repository interface with category-specific operations
/// </summary>
public interface ISkillCategoryRepository : IRepository<SkillCategory>
{
    /// <summary>
    /// Get category by name
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category if found, null otherwise</returns>
    Task<SkillCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get category by slug
    /// </summary>
    /// <param name="slug">Category slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category if found, null otherwise</returns>
    Task<SkillCategory?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get active categories only
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Active categories</returns>
    Task<IEnumerable<SkillCategory>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get category with its skills
    /// </summary>
    /// <param name="categoryId">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category with skills if found, null otherwise</returns>
    Task<SkillCategory?> GetWithSkillsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get categories ordered by display order
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Categories ordered by display order</returns>
    Task<IEnumerable<SkillCategory>> GetOrderedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if category name exists
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="excludeId">ID to exclude from check (for updates)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if name exists, false otherwise</returns>
    Task<bool> NameExistsAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    );
}
