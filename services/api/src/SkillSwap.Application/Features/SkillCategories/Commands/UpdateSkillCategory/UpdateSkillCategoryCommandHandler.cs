using Microsoft.Extensions.Logging;
using SkillSwap.Application.Common;
using SkillSwap.Application.Interfaces;
using SkillSwap.Contracts.Responses;

namespace SkillSwap.Application.Features.SkillCategories.Commands.UpdateSkillCategory;

/// <summary>
/// Handler for updating an existing skill category
/// </summary>
public sealed class UpdateSkillCategoryCommandHandler
    : ICommandHandler<UpdateSkillCategoryCommand, SkillCategoryResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSkillCategoryCommandHandler> _logger;

    public UpdateSkillCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<UpdateSkillCategoryCommandHandler> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SkillCategoryResponse> Handle(
        UpdateSkillCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Updating skill category with ID: {CategoryId}", request.Id);

        // Get the existing category
        var category = await _unitOfWork.SkillCategories.GetByIdAsync(
            request.Id,
            cancellationToken
        );
        if (category == null)
        {
            throw new KeyNotFoundException($"Skill category with ID {request.Id} not found.");
        }

        // Check if another category with the same name exists (excluding current category)
        var existingCategory = await _unitOfWork.SkillCategories.GetByNameAsync(
            request.Name,
            cancellationToken
        );

        if (existingCategory != null && existingCategory.Id != request.Id)
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }

        // Update the category properties
        category.UpdateCategory(
            name: request.Name,
            description: request.Description,
            color: request.ColorHex,
            icon: request.IconUrl
        );

        // Update in repository and save
        _unitOfWork.SkillCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully updated skill category {CategoryId} with name: {CategoryName}",
            category.Id,
            category.Name
        );

        // Return response
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
