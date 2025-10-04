using FluentValidation;

namespace SkillSwap.Application.Features.SkillCategories.Commands.DeleteSkillCategory;

/// <summary>
/// Validator for DeleteSkillCategoryCommand
/// </summary>
public sealed class DeleteSkillCategoryCommandValidator
    : AbstractValidator<DeleteSkillCategoryCommand>
{
    public DeleteSkillCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Category ID is required");
    }
}
