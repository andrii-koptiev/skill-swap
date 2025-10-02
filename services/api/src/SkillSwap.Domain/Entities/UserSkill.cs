using SkillSwap.Domain.Common;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Represents a skill that a user can teach or wants to learn
/// </summary>
public class UserSkill : BaseEntity
{
    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private UserSkill() { }

    /// <summary>
    /// Creates a new UserSkill association
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="skillId">The ID of the skill</param>
    /// <param name="skillType">Whether the user can teach or wants to learn this skill</param>
    /// <param name="proficiencyLevel">User's proficiency level (1-5 scale)</param>
    /// <param name="yearsOfExperience">Optional years of experience</param>
    /// <param name="description">Optional description of the user's experience</param>
    /// <param name="isPrimary">Whether this is a primary skill for the user</param>
    public UserSkill(
        Guid userId,
        Guid skillId,
        SkillType skillType,
        int proficiencyLevel,
        int? yearsOfExperience = null,
        string? description = null,
        bool isPrimary = false
    )
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        if (skillId == Guid.Empty)
            throw new ArgumentException("Skill ID cannot be empty", nameof(skillId));

        Id = Guid.NewGuid();
        UserId = userId;
        SkillId = skillId;
        SkillType = skillType;
        ProficiencyLevel = ValidateProficiencyLevel(proficiencyLevel);
        YearsOfExperience = yearsOfExperience;
        Description = description?.Trim();
        IsPrimary = isPrimary;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// The ID of the user who has this skill
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// The ID of the skill
    /// </summary>
    public Guid SkillId { get; private set; }

    /// <summary>
    /// Whether the user can teach or wants to learn this skill
    /// </summary>
    public SkillType SkillType { get; private set; }

    /// <summary>
    /// User's proficiency level (1-5 scale)
    /// 1 = Beginner, 2 = Novice, 3 = Intermediate, 4 = Advanced, 5 = Expert
    /// </summary>
    public int ProficiencyLevel { get; private set; }

    /// <summary>
    /// Optional years of experience with this skill
    /// </summary>
    public int? YearsOfExperience { get; private set; }

    /// <summary>
    /// Optional description of the user's experience with this skill
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Whether this is a primary skill for the user
    /// </summary>
    public bool IsPrimary { get; private set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Navigation property to the skill
    /// </summary>
    public Skill Skill { get; private set; } = null!;

    /// <summary>
    /// Updates the proficiency level and experience
    /// </summary>
    /// <param name="proficiencyLevel">New proficiency level</param>
    /// <param name="yearsOfExperience">New years of experience</param>
    public void UpdateProficiency(int proficiencyLevel, int? yearsOfExperience = null)
    {
        ProficiencyLevel = ValidateProficiencyLevel(proficiencyLevel);
        YearsOfExperience = yearsOfExperience;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Updates the description
    /// </summary>
    /// <param name="description">New description</param>
    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Sets this skill as a primary skill for the user
    /// </summary>
    public void SetAsPrimary()
    {
        IsPrimary = true;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Removes primary skill designation
    /// </summary>
    public void RemovePrimaryDesignation()
    {
        IsPrimary = false;
        base.UpdateTimestamp();
    }

    /// <summary>
    /// Gets the proficiency description for this user skill
    /// </summary>
    /// <returns>Proficiency level description</returns>
    public string GetProficiencyDescription()
    {
        return ProficiencyLevel switch
        {
            1 => "Beginner",
            2 => "Novice",
            3 => "Intermediate",
            4 => "Advanced",
            5 => "Expert",
            _ => "Unknown",
        };
    }

    /// <summary>
    /// Validates proficiency level is within acceptable range
    /// </summary>
    /// <param name="level">Proficiency level to validate</param>
    /// <returns>Valid proficiency level</returns>
    /// <exception cref="ArgumentException">When level is outside 1-5 range</exception>
    private static int ValidateProficiencyLevel(int level)
    {
        if (level < 1 || level > 5)
            throw new ArgumentException("Proficiency level must be between 1 and 5", nameof(level));
        return level;
    }
}
