using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<Role?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Role?> GetWithPermissionsAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> NameExistsAsync(
        string name,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbSet.Where(r => r.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(r => r.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken).ConfigureAwait(false);
    }
}
