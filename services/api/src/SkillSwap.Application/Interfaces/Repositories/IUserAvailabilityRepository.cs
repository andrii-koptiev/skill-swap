using SkillSwap.Domain.Entities;

namespace SkillSwap.Application.Interfaces.Repositories;

/// <summary>
/// User Availability repository interface
/// </summary>
public interface IUserAvailabilityRepository : IRepository<UserAvailability>
{
    /// <summary>
    /// Get availability by user ID
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User availability records</returns>
    Task<IEnumerable<UserAvailability>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get availability by day of week
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dayOfWeek">Day of week</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User availability for the specified day</returns>
    Task<UserAvailability?> GetByUserAndDayAsync(
        Guid userId,
        DayOfWeek dayOfWeek,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Get users available at specific time
    /// </summary>
    /// <param name="dayOfWeek">Day of week</param>
    /// <param name="timeSlot">Time slot</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Users available at the specified time</returns>
    Task<IEnumerable<UserAvailability>> GetUsersAvailableAtTimeAsync(
        DayOfWeek dayOfWeek,
        TimeOnly timeSlot,
        CancellationToken cancellationToken = default
    );
}
