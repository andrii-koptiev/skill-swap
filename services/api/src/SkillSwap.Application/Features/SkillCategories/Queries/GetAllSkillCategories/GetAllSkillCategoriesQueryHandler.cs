using Microsoft.Extensions.Logging;
using SkillSwap.Application.Common;
using SkillSwap.Application.Interfaces;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Application.Features.SkillCategories.Queries.GetAllSkillCategories;

/// <summary>
/// Handler for getting all active skill categories
/// </summary>
public sealed class GetAllSkillCategoriesQueryHandler
    : IQueryHandler<GetAllSkillCategoriesQuery, IEnumerable<SkillCategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAllSkillCategoriesQueryHandler> _logger;

    public GetAllSkillCategoriesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetAllSkillCategoriesQueryHandler> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<SkillCategoryResponse>> Handle(
        GetAllSkillCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Getting all active skill categories");

        var categories = await _unitOfWork.SkillCategories.GetActiveAsync(cancellationToken);

        var responses = categories.Select(category => new SkillCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description ?? string.Empty,
            ColorHex = category.Color ?? string.Empty,
            IconUrl = category.Icon,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        });

        _logger.LogInformation(
            "Retrieved {CategoryCount} active skill categories",
            responses.Count()
        );

        return responses;
    }
}
