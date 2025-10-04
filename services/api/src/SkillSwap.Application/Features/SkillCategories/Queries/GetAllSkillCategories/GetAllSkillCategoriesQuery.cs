using SkillSwap.Application.Common;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Application.Features.SkillCategories.Queries.GetAllSkillCategories;

/// <summary>
/// Query to get all active skill categories
/// </summary>
public sealed record GetAllSkillCategoriesQuery : IQuery<IEnumerable<SkillCategoryResponse>> { }
