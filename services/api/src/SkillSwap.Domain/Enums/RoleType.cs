namespace SkillSwap.Domain.Enums;

/// <summary>
/// Defines the different types of roles in the SkillSwap system
/// </summary>
public enum RoleType
{
    /// <summary>
    /// Super administrator with full system access
    /// </summary>
    SuperAdmin = 1,

    /// <summary>
    /// Administrator with administrative privileges
    /// </summary>
    Administrator = 2,

    /// <summary>
    /// Moderator with content moderation privileges
    /// </summary>
    Moderator = 3,

    /// <summary>
    /// Regular user with standard permissions
    /// </summary>
    User = 4,
}
