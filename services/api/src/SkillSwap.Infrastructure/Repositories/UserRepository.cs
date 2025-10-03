using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.ValueObjects;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

/// <summary>
/// User repository implementation with user-specific operations
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<User?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> EmailExistsAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> UsernameExistsAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .AnyAsync(u => u.Username == username, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<User>> GetUsersBySkillsAsync(
        IEnumerable<Guid> skillIds,
        CancellationToken cancellationToken = default
    )
    {
        var skillIdsList = skillIds.ToList();
        if (!skillIdsList.Any())
            return Enumerable.Empty<User>();

        return await _dbSet
            .Where(u => u.UserSkills.Any(us => skillIdsList.Contains(us.SkillId)))
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<User?> GetWithSkillsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Include(u => u.UserSkills)
            .ThenInclude(us => us.Skill)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<User?> GetWithAvailabilityAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Include(u => u.Availability)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            .ConfigureAwait(false);
    }
}
