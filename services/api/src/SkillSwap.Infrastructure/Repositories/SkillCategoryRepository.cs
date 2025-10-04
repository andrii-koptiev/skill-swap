using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

/// <summary>
/// Skill Category repository implementation with category-specific operations
/// </summary>
public class SkillCategoryRepository : Repository<SkillCategory>, ISkillCategoryRepository
{
    public SkillCategoryRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<SkillCategory?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<SkillCategory?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SkillCategory>> GetActiveAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<SkillCategory?> GetWithSkillsAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Include(c => c.Skills.Where(s => s.IsActive))
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<SkillCategory>> GetOrderedAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbSet.Where(c => c.Name == name);

        if (excludeId is not null)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }
}
