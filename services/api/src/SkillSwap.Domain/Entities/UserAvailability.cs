using SkillSwap.Domain.Common;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Represents user availability for skill swap sessions
/// </summary>
public class UserAvailability : BaseEntity
{
    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private UserAvailability() { }

    /// <summary>
    /// Creates a new user availability entry
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="dayOfWeek">Day of the week</param>
    /// <param name="startTime">Start time for availability</param>
    /// <param name="endTime">End time for availability</param>
    /// <param name="timezone">Timezone for the availability</param>
    public UserAvailability(
        Guid userId,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        string timezone
    )
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException("Timezone cannot be null or empty", nameof(timezone));
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        Id = Guid.NewGuid();
        UserId = userId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        Timezone = timezone.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The ID of the user this availability belongs to
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Day of the week for this availability
    /// </summary>
    public DayOfWeek DayOfWeek { get; private set; }

    /// <summary>
    /// Start time for the availability window
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// End time for the availability window
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// Timezone for the availability times
    /// </summary>
    public string Timezone { get; private set; } = string.Empty;

    /// <summary>
    /// Whether this availability slot is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Updates the time slot for this availability
    /// </summary>
    /// <param name="startTime">New start time</param>
    /// <param name="endTime">New end time</param>
    public void UpdateTimeSlot(TimeOnly startTime, TimeOnly endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        StartTime = startTime;
        EndTime = endTime;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Updates the timezone for this availability
    /// </summary>
    /// <param name="timezone">New timezone</param>
    public void UpdateTimezone(string timezone)
    {
        if (string.IsNullOrWhiteSpace(timezone))
            throw new ArgumentException("Timezone cannot be null or empty", nameof(timezone));

        Timezone = timezone.Trim();
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Activates this availability slot
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Deactivates this availability slot
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Checks if the availability slot overlaps with another time window
    /// </summary>
    /// <param name="otherStart">Start time of the other window</param>
    /// <param name="otherEnd">End time of the other window</param>
    /// <returns>True if there is overlap</returns>
    public bool OverlapsWith(TimeOnly otherStart, TimeOnly otherEnd)
    {
        return StartTime < otherEnd && EndTime > otherStart;
    }

    /// <summary>
    /// Gets the duration of this availability slot in minutes
    /// </summary>
    /// <returns>Duration in minutes</returns>
    public int GetDurationInMinutes()
    {
        return (int)(EndTime - StartTime).TotalMinutes;
    }
}
