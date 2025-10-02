using SkillSwap.Domain.Common;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// User preferences and settings
/// </summary>
public class UserPreferences : BaseEntity
{
    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private UserPreferences() { }

    /// <summary>
    /// Creates a new user preferences with default settings
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    public UserPreferences(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        Id = Guid.NewGuid();
        UserId = userId;

        // Set default preferences
        EmailNotifications = true;
        PushNotifications = true;
        SessionReminders = true;
        MarketingEmails = false;
        PreferredSessionDuration = 60;
        PreferredMeetingPlatform = "built_in";
        AutoAcceptFromVerified = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The ID of the user these preferences belong to
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Whether to receive email notifications
    /// </summary>
    public bool EmailNotifications { get; private set; }

    /// <summary>
    /// Whether to receive push notifications
    /// </summary>
    public bool PushNotifications { get; private set; }

    /// <summary>
    /// Whether to receive session reminder notifications
    /// </summary>
    public bool SessionReminders { get; private set; }

    /// <summary>
    /// Whether to receive marketing emails
    /// </summary>
    public bool MarketingEmails { get; private set; }

    /// <summary>
    /// Preferred session duration in minutes
    /// </summary>
    public int PreferredSessionDuration { get; private set; }

    /// <summary>
    /// Maximum travel distance for in-person sessions (optional)
    /// </summary>
    public int? MaxTravelDistance { get; private set; }

    /// <summary>
    /// Preferred meeting platform for sessions
    /// </summary>
    public string PreferredMeetingPlatform { get; private set; } = string.Empty;

    /// <summary>
    /// Whether to automatically accept session requests from verified users
    /// </summary>
    public bool AutoAcceptFromVerified { get; private set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Updates notification preferences
    /// </summary>
    /// <param name="email">Enable email notifications</param>
    /// <param name="push">Enable push notifications</param>
    /// <param name="sessionReminders">Enable session reminders</param>
    /// <param name="marketing">Enable marketing emails</param>
    public void UpdateNotificationPreferences(
        bool email,
        bool push,
        bool sessionReminders,
        bool marketing
    )
    {
        EmailNotifications = email;
        PushNotifications = push;
        SessionReminders = sessionReminders;
        MarketingEmails = marketing;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Updates session preferences
    /// </summary>
    /// <param name="sessionDuration">Preferred session duration in minutes</param>
    /// <param name="meetingPlatform">Preferred meeting platform</param>
    /// <param name="autoAcceptFromVerified">Auto-accept from verified users</param>
    public void UpdateSessionPreferences(
        int sessionDuration,
        string meetingPlatform,
        bool autoAcceptFromVerified
    )
    {
        if (sessionDuration <= 0)
            throw new ArgumentException(
                "Session duration must be positive",
                nameof(sessionDuration)
            );
        if (string.IsNullOrWhiteSpace(meetingPlatform))
            throw new ArgumentException(
                "Meeting platform cannot be null or empty",
                nameof(meetingPlatform)
            );

        PreferredSessionDuration = sessionDuration;
        PreferredMeetingPlatform = meetingPlatform.Trim();
        AutoAcceptFromVerified = autoAcceptFromVerified;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Updates the maximum travel distance for in-person sessions
    /// </summary>
    /// <param name="maxDistance">Maximum travel distance in appropriate units</param>
    public void UpdateMaxTravelDistance(int? maxDistance)
    {
        if (maxDistance.HasValue && maxDistance.Value < 0)
            throw new ArgumentException(
                "Max travel distance cannot be negative",
                nameof(maxDistance)
            );

        MaxTravelDistance = maxDistance;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Resets all preferences to default values
    /// </summary>
    public void ResetToDefaults()
    {
        EmailNotifications = true;
        PushNotifications = true;
        SessionReminders = true;
        MarketingEmails = false;
        PreferredSessionDuration = 60;
        PreferredMeetingPlatform = "built_in";
        AutoAcceptFromVerified = false;
        MaxTravelDistance = null;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Gets a summary of current notification settings
    /// </summary>
    /// <returns>Summary string of notification preferences</returns>
    public string GetNotificationSummary()
    {
        var notifications = new List<string>();
        if (EmailNotifications)
            notifications.Add("Email");
        if (PushNotifications)
            notifications.Add("Push");
        if (SessionReminders)
            notifications.Add("Reminders");
        if (MarketingEmails)
            notifications.Add("Marketing");

        return notifications.Count > 0 ? string.Join(", ", notifications) : "None";
    }
}
