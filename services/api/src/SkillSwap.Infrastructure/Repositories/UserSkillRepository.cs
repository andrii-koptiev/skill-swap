using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

public class UserSkillRepository : Repository<UserSkill>, IUserSkillRepository
{
    public UserSkillRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<IEnumerable<UserSkill>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(us => us.UserId == userId)
            .Include(us => us.Skill)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<UserSkill>> GetBySkillIdAsync(
        Guid skillId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(us => us.SkillId == skillId)
            .Include(us => us.User)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<UserSkill?> GetByUserAndSkillAsync(
        Guid userId,
        Guid skillId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                us => us.UserId == userId && us.SkillId == skillId,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<UserSkill>> GetByProficiencyAsync(
        Guid userId,
        int proficiency,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(us => us.UserId == userId && us.ProficiencyLevel == proficiency)
            .Include(us => us.Skill)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<UserSkill>> GetTeachableSkillsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(us => us.UserId == userId && us.SkillType == SkillType.CanTeach)
            .Include(us => us.Skill)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<UserSkill>> GetLearningSkillsAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(us => us.UserId == userId && us.SkillType == SkillType.WantToLearn)
            .Include(us => us.Skill)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<bool> UserHasSkillAsync(
        Guid userId,
        Guid skillId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .AnyAsync(us => us.UserId == userId && us.SkillId == skillId, cancellationToken)
            .ConfigureAwait(false);
    }
}
