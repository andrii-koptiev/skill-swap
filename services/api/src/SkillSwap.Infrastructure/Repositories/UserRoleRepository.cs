using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(ur => ur.RoleId == roleId)
            .Include(ur => ur.User)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<UserRole?> GetByUserAndRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                ur => ur.UserId == userId && ur.RoleId == roleId,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task<bool> UserHasRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> UserHasRoleAsync(
        Guid userId,
        string roleName,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName, cancellationToken)
            .ConfigureAwait(false);
    }
}
