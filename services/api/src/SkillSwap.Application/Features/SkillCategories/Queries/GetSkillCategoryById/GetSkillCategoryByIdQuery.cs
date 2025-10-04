using SkillSwap.Application.Common;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Application.Features.SkillCategories.Queries.GetSkillCategoryById;

/// <summary>
/// Query to get a skill category by its ID
/// </summary>
public sealed record GetSkillCategoryByIdQuery : IQuery<SkillCategoryResponse?>
{
    /// <summary>
    /// The ID of the skill category to retrieve
    /// </summary>
    public Guid Id { get; init; }
}
