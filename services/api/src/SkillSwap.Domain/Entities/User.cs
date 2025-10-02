using SkillSwap.Domain.Common;
using SkillSwap.Domain.Enums;
using SkillSwap.Domain.ValueObjects;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// User aggregate root representing a platform user
/// </summary>
public class User : BaseEntity
{
    private readonly List<UserSkill> _userSkills = [];
    private readonly List<UserAvailability> _availability = [];

    // Private parameterless constructor for EF Core
    private User() { }

    public User(Email email, string username, string passwordHash, UserProfile profile)
        : base()
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Username = ValidateUsername(username);
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
        Status = UserStatus.PendingVerification;
        VerificationStatus = VerificationStatus.Unverified;
        LastLoginAt = null;
    }

    /// <summary>
    /// User's unique email address
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// User's unique username
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// Hashed password for authentication
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// User profile information
    /// </summary>
    public UserProfile Profile { get; private set; } = null!;

    /// <summary>
    /// Current user account status
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// User verification status
    /// </summary>
    public VerificationStatus VerificationStatus { get; private set; }

    /// <summary>
    /// Last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; private set; }

    /// <summary>
    /// Collection of user's skills (can teach or want to learn)
    /// </summary>
    public IReadOnlyCollection<UserSkill> UserSkills => _userSkills.AsReadOnly();

    /// <summary>
    /// User's availability for skill swap sessions
    /// </summary>
    public IReadOnlyCollection<UserAvailability> Availability => _availability.AsReadOnly();

    /// <summary>
    /// User preferences (lazy loaded)
    /// </summary>
    public UserPreferences? Preferences { get; private set; }

    // Public methods for business operations

    /// <summary>
    /// Updates the user's profile information
    /// </summary>
    public void UpdateProfile(UserProfile newProfile)
    {
        Profile = newProfile ?? throw new ArgumentNullException(nameof(newProfile));
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the user's email address
    /// </summary>
    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail ?? throw new ArgumentNullException(nameof(newEmail));
        VerificationStatus = VerificationStatus.Unverified; // Reset verification when email changes
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the user's password hash
    /// </summary>
    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException(
                "Password hash cannot be null or empty",
                nameof(newPasswordHash)
            );

        PasswordHash = newPasswordHash;
        UpdateTimestamp();
    }

    /// <summary>
    /// Activates the user account
    /// </summary>
    public void Activate()
    {
        Status = UserStatus.Active;
        UpdateTimestamp();
    }

    /// <summary>
    /// Deactivates the user account
    /// </summary>
    public void Deactivate()
    {
        Status = UserStatus.Inactive;
        UpdateTimestamp();
    }

    /// <summary>
    /// Suspends the user account
    /// </summary>
    public void Suspend()
    {
        Status = UserStatus.Suspended;
        UpdateTimestamp();
    }

    /// <summary>
    /// Bans the user account permanently
    /// </summary>
    public void Ban()
    {
        Status = UserStatus.Banned;
        UpdateTimestamp();
    }

    /// <summary>
    /// Verifies the user's email
    /// </summary>
    public void VerifyEmail()
    {
        VerificationStatus = VerificationStatus.Verified;
        if (Status == UserStatus.PendingVerification)
        {
            Status = UserStatus.Active;
        }
        UpdateTimestamp();
    }

    /// <summary>
    /// Records a successful login
    /// </summary>
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    /// <summary>
    /// Adds a skill to the user's profile
    /// </summary>
    public void AddSkill(UserSkill userSkill)
    {
        if (userSkill == null)
            throw new ArgumentNullException(nameof(userSkill));

        if (
            _userSkills.Any(us =>
                us.SkillId == userSkill.SkillId && us.SkillType == userSkill.SkillType
            )
        )
            throw new InvalidOperationException("User already has this skill with the same type");

        _userSkills.Add(userSkill);
        UpdateTimestamp();
    }

    /// <summary>
    /// Removes a skill from the user's profile
    /// </summary>
    public void RemoveSkill(Guid skillId, SkillType skillType)
    {
        var userSkill = _userSkills.FirstOrDefault(us =>
            us.SkillId == skillId && us.SkillType == skillType
        );
        if (userSkill != null)
        {
            _userSkills.Remove(userSkill);
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Updates availability for the user
    /// </summary>
    public void UpdateAvailability(IEnumerable<UserAvailability> newAvailability)
    {
        _availability.Clear();
        _availability.AddRange(
            newAvailability ?? throw new ArgumentNullException(nameof(newAvailability))
        );
        UpdateTimestamp();
    }

    /// <summary>
    /// Sets user preferences
    /// </summary>
    public void SetPreferences(UserPreferences preferences)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        UpdateTimestamp();
    }

    /// <summary>
    /// Checks if user can teach a specific skill
    /// </summary>
    public bool CanTeach(Guid skillId)
    {
        return _userSkills.Any(us => us.SkillId == skillId && us.SkillType == SkillType.CanTeach);
    }

    /// <summary>
    /// Checks if user wants to learn a specific skill
    /// </summary>
    public bool WantsToLearn(Guid skillId)
    {
        return _userSkills.Any(us =>
            us.SkillId == skillId && us.SkillType == SkillType.WantToLearn
        );
    }

    /// <summary>
    /// Gets user's skills they can teach
    /// </summary>
    public IEnumerable<UserSkill> GetTeachableSkills()
    {
        return _userSkills.Where(us => us.SkillType == SkillType.CanTeach);
    }

    /// <summary>
    /// Gets user's skills they want to learn
    /// </summary>
    public IEnumerable<UserSkill> GetLearningSkills()
    {
        return _userSkills.Where(us => us.SkillType == SkillType.WantToLearn);
    }

    /// <summary>
    /// Checks if user is available for sessions
    /// </summary>
    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }

    /// <summary>
    /// Checks if user's email is verified
    /// </summary>
    public bool IsEmailVerified()
    {
        return VerificationStatus >= VerificationStatus.Verified;
    }

    // Private helper methods

    private static string ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty", nameof(username));

        if (username.Length < 3 || username.Length > 50)
            throw new ArgumentException(
                "Username must be between 3 and 50 characters",
                nameof(username)
            );

        if (!System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9_-]+$"))
            throw new ArgumentException(
                "Username can only contain letters, numbers, underscores, and hyphens",
                nameof(username)
            );

        return username.Trim().ToLowerInvariant();
    }
}
