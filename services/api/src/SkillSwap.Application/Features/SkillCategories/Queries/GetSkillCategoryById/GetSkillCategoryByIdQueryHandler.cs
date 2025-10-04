using Microsoft.Extensions.Logging;
using SkillSwap.Application.Common;
using SkillSwap.Application.Interfaces;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Application.Features.SkillCategories.Queries.GetSkillCategoryById;

/// <summary>
/// Handler for getting a skill category by its ID
/// </summary>
public sealed class GetSkillCategoryByIdQueryHandler
    : IQueryHandler<GetSkillCategoryByIdQuery, SkillCategoryResponse?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSkillCategoryByIdQueryHandler> _logger;

    public GetSkillCategoryByIdQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetSkillCategoryByIdQueryHandler> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillCategoryResponse?> Handle(
        GetSkillCategoryByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Getting skill category with ID: {CategoryId}", request.Id);

        var category = await _unitOfWork.SkillCategories.GetByIdAsync(
            request.Id,
            cancellationToken
        );

        if (category == null)
        {
            _logger.LogWarning("Skill category with ID {CategoryId} not found", request.Id);
            return null;
        }

        _logger.LogInformation("Successfully retrieved skill category {CategoryId}", request.Id);

        return new SkillCategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description ?? string.Empty,
            ColorHex = category.Color ?? string.Empty,
            IconUrl = category.Icon,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt,
        };
    }
}
