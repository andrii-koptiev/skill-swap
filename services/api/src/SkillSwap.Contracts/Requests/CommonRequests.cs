namespace SkillSwap.Contracts.Requests;

/// <summary>
/// Request to create a new skill category.
/// </summary>
public sealed record CreateSkillCategoryRequest
{
    /// <summary>
    /// The name of the skill category.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// A brief description of the skill category.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Hex color code for the category (e.g., #FF5733).
    /// </summary>
    public string ColorHex { get; init; } = string.Empty;

    /// <summary>
    /// URL to the category icon.
    /// </summary>
    public string? IconUrl { get; init; }
}

/// <summary>
/// Request to create a new skill.
/// </summary>
public sealed record CreateSkillRequest
{
    /// <summary>
    /// The name of the skill.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// A detailed description of the skill.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// The ID of the skill category this skill belongs to.
    /// </summary>
    public Guid CategoryId { get; init; }
}

/// <summary>
/// Request to register a new user.
/// </summary>
public sealed record RegisterUserRequest
{
    /// <summary>
    /// User's email address.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// User's first name.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// User's last name.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// User's password.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Password confirmation.
    /// </summary>
    public string ConfirmPassword { get; init; } = string.Empty;

    /// <summary>
    /// User's timezone (e.g., "America/New_York").
    /// </summary>
    public string TimeZone { get; init; } = "UTC";

    /// <summary>
    /// User's preferred language code (e.g., "en-US").
    /// </summary>
    public string PreferredLanguage { get; init; } = "en-US";
}
