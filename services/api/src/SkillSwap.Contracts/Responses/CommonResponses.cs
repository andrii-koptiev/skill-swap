namespace SkillSwap.Contracts.Responses;

/// <summary>
/// Response DTO for skill category data.
/// </summary>
public sealed record SkillCategoryResponse
{
    /// <summary>
    /// The unique identifier of the skill category.
    /// </summary>
    public Guid Id { get; init; }

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

    /// <summary>
    /// When the skill category was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the skill category was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}

/// <summary>
/// Response DTO for skill data.
/// </summary>
public sealed record SkillResponse
{
    /// <summary>
    /// The unique identifier of the skill.
    /// </summary>
    public Guid Id { get; init; }

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

    /// <summary>
    /// The skill category information.
    /// </summary>
    public SkillCategoryResponse? Category { get; init; }

    /// <summary>
    /// When the skill was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the skill was last updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}
