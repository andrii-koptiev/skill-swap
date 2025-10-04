using Microsoft.Extensions.Logging;
using SkillSwap.Application.Common;
using SkillSwap.Application.Interfaces;

namespace SkillSwap.Application.Features.SkillCategories.Commands.DeleteSkillCategory;

/// <summary>
/// Handler for deleting a skill category
/// </summary>
public sealed class DeleteSkillCategoryCommandHandler : ICommandHandler<DeleteSkillCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSkillCategoryCommandHandler> _logger;

    public DeleteSkillCategoryCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteSkillCategoryCommandHandler> logger
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(
        DeleteSkillCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation("Deleting skill category with ID: {CategoryId}", request.Id);

        // Get the existing category
        var category = await _unitOfWork.SkillCategories.GetByIdAsync(
            request.Id,
            cancellationToken
        );
        if (category == null)
        {
            throw new KeyNotFoundException($"Skill category with ID {request.Id} not found.");
        }

        // Check if category has associated skills
        var skillsInCategory = await _unitOfWork.Skills.GetByCategoryAsync(
            request.Id,
            cancellationToken
        );
        if (skillsInCategory.Any())
        {
            throw new InvalidOperationException(
                "Cannot delete category that contains skills. Move or delete associated skills first."
            );
        }

        // Soft delete the category (set IsActive to false)
        category.Deactivate();
        _unitOfWork.SkillCategories.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully deactivated skill category {CategoryId} with name: {CategoryName}",
            category.Id,
            category.Name
        );
    }
}
