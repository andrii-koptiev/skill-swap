using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

/// <summary>
/// Skill repository implementation with skill-specific operations
/// </summary>
public class SkillRepository : Repository<Skill>, ISkillRepository
{
    public SkillRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<Skill?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Skill?> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Slug == slug, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<Skill>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(s => s.CategoryId == categoryId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<Skill>> GetActiveSkillsAsync(
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<Skill>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Enumerable.Empty<Skill>();

        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(s =>
                s.IsActive
                && (
                    s.Name.ToLower().Contains(lowerSearchTerm)
                    || (s.Description != null && s.Description.ToLower().Contains(lowerSearchTerm))
                )
            )
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> NameExistsInCategoryAsync(
        string name,
        Guid categoryId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbSet.Where(s => s.Name == name && s.CategoryId == categoryId);

        if (excludeId.HasValue)
        {
            query = query.Where(s => s.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }
}
