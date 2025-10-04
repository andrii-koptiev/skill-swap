using SkillSwap.Application.Common;

namespace SkillSwap.Application.Features.SkillCategories.Commands.DeleteSkillCategory;

/// <summary>
/// Command to delete a skill category
/// </summary>
public sealed record DeleteSkillCategoryCommand : ICommand
{
    /// <summary>
    /// The ID of the skill category to delete
    /// </summary>
    public Guid Id { get; init; }
}
