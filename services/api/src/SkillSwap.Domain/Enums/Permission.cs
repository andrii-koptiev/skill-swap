namespace SkillSwap.Domain.Enums;

/// <summary>
/// Defines the different permissions available in the SkillSwap system
/// </summary>
public enum Permission
{
    // User Management
    ViewUsers = 1,
    CreateUsers = 2,
    UpdateUsers = 3,
    DeleteUsers = 4,

    // Skill Management
    ViewSkills = 5,
    CreateSkills = 6,
    UpdateSkills = 7,
    DeleteSkills = 8,

    // Content Moderation
    ModerateContent = 9,
    ViewReports = 10,
    ResolveReports = 11,

    // System Administration
    ManageSystem = 12,
    ViewSystemLogs = 13,
    ManageRoles = 14,

    // Swap/Exchange Management
    ViewSwaps = 15,
    CreateSwaps = 16,
    UpdateSwaps = 17,
    DeleteSwaps = 18,

    // Profile Management
    ViewProfiles = 19,
    UpdateOwnProfile = 20,
    UpdateAnyProfile = 21,
}
