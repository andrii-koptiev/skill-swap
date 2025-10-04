using SkillSwap.Application.Common;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Application.Features.SkillCategories.Commands.CreateSkillCategory;

/// <summary>
/// Command to create a new skill category
/// </summary>
public sealed record CreateSkillCategoryCommand : ICommand<SkillCategoryResponse>
{
    /// <summary>
    /// The name of the skill category
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// A brief description of the skill category
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Hex color code for the category (e.g., #FF5733)
    /// </summary>
    public string ColorHex { get; init; } = string.Empty;

    /// <summary>
    /// URL to the category icon
    /// </summary>
    public string? IconUrl { get; init; }
}
