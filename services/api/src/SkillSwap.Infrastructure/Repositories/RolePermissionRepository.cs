using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
{
    public RolePermissionRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<IEnumerable<RolePermission>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<RolePermission>> GetByPermissionAsync(
        string permission,
        CancellationToken cancellationToken = default
    )
    {
        if (Enum.TryParse<Permission>(permission, out var permissionEnum))
        {
            return await _dbSet
                .Where(rp => rp.Permission == permissionEnum)
                .Include(rp => rp.Role)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        return Enumerable.Empty<RolePermission>();
    }

    public async Task<RolePermission?> GetByRoleAndPermissionAsync(
        Guid roleId,
        string permission,
        CancellationToken cancellationToken = default
    )
    {
        if (Enum.TryParse<Permission>(permission, out var permissionEnum))
        {
            return await _dbSet
                .FirstOrDefaultAsync(
                    rp => rp.RoleId == roleId && rp.Permission == permissionEnum,
                    cancellationToken
                )
                .ConfigureAwait(false);
        }

        return null;
    }

    public async Task<bool> RoleHasPermissionAsync(
        Guid roleId,
        string permission,
        CancellationToken cancellationToken = default
    )
    {
        if (Enum.TryParse<Permission>(permission, out var permissionEnum))
        {
            return await _dbSet
                .AnyAsync(
                    rp => rp.RoleId == roleId && rp.Permission == permissionEnum,
                    cancellationToken
                )
                .ConfigureAwait(false);
        }

        return false;
    }
}
