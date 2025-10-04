using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

public class UserPreferencesRepository : Repository<UserPreferences>, IUserPreferencesRepository
{
    public UserPreferencesRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<UserPreferences?> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken)
            .ConfigureAwait(false);
    }
}
