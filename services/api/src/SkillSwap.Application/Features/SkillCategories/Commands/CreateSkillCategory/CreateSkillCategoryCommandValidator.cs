using FluentValidation;
using SkillSwap.Application.Features.SkillCategories.Commands.CreateSkillCategory;

namespace SkillSwap.Application.Features.SkillCategories.Commands.CreateSkillCategory;

/// <summary>
/// Validator for CreateSkillCategoryCommand
/// </summary>
public sealed class CreateSkillCategoryCommandValidator
    : AbstractValidator<CreateSkillCategoryCommand>
{
    public CreateSkillCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(50)
            .WithMessage("Category name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&]+$")
            .WithMessage(
                "Category name can only contain letters, numbers, spaces, hyphens, and ampersands"
            );

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Category description is required")
            .MaximumLength(500)
            .WithMessage("Category description must not exceed 500 characters");

        RuleFor(x => x.ColorHex)
            .NotEmpty()
            .WithMessage("Color hex code is required")
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Color hex code must be in valid format (e.g., #FF5733 or #F53)");

        RuleFor(x => x.IconUrl)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrEmpty(x.IconUrl))
            .WithMessage("Icon URL must be a valid URL");
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
