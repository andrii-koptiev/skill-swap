namespace SkillSwap.Domain.Enums;

/// <summary>
/// Skill categories for organizing skills on the platform
/// </summary>
public enum SkillCategory
{
    /// <summary>
    /// Technology and programming skills
    /// </summary>
    Technology = 1,

    /// <summary>
    /// Creative and artistic skills
    /// </summary>
    Creative = 2,

    /// <summary>
    /// Business and professional skills
    /// </summary>
    Business = 3,

    /// <summary>
    /// Language and communication skills
    /// </summary>
    Language = 4,

    /// <summary>
    /// Health and fitness skills
    /// </summary>
    Health = 5,

    /// <summary>
    /// Culinary and cooking skills
    /// </summary>
    Culinary = 6,

    /// <summary>
    /// Crafts and hobby skills
    /// </summary>
    Crafts = 7,

    /// <summary>
    /// Education and teaching skills
    /// </summary>
    Education = 8,

    /// <summary>
    /// Music and musical skills
    /// </summary>
    Music = 9,

    /// <summary>
    /// Sports and recreation skills
    /// </summary>
    Sports = 10,

    /// <summary>
    /// Science and research skills
    /// </summary>
    Science = 11,

    /// <summary>
    /// Other skills not covered by above categories
    /// </summary>
    Other = 12,
}

/// <summary>
/// Skill type enumeration - how a user relates to a skill
/// </summary>
public enum SkillType
{
    /// <summary>
    /// User can teach this skill to others
    /// </summary>
    CanTeach = 1,

    /// <summary>
    /// User wants to learn this skill from others
    /// </summary>
    WantToLearn = 2,
}
