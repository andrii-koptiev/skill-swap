using Microsoft.EntityFrameworkCore;
using SkillSwap.Application.Interfaces.Repositories;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Repositories;

public class UserAvailabilityRepository : Repository<UserAvailability>, IUserAvailabilityRepository
{
    public UserAvailabilityRepository(SkillSwapDbContext context)
        : base(context) { }

    public async Task<IEnumerable<UserAvailability>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(ua => ua.UserId == userId)
            .OrderBy(ua => ua.DayOfWeek)
            .ThenBy(ua => ua.StartTime)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<UserAvailability?> GetByUserAndDayAsync(
        Guid userId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                ua => ua.UserId == userId && ua.DayOfWeek == dayOfWeek,
                cancellationToken
            )
            .ConfigureAwait(false);
    }

    public async Task<IEnumerable<UserAvailability>> GetUsersAvailableAtTimeAsync(
        DayOfWeek dayOfWeek,
        TimeOnly timeSlot,
        CancellationToken cancellationToken = default
    )
    {
        return await _dbSet
            .Where(ua =>
                ua.DayOfWeek == dayOfWeek && ua.StartTime <= timeSlot && ua.EndTime >= timeSlot
            )
            .Include(ua => ua.User)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}
