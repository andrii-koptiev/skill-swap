using System.Text.RegularExpressions;
using FluentValidation;
using SkillSwap.Contracts.Requests;

namespace SkillSwap.Api.Validators;

/// <summary>
/// Validator for CreateSkillCategoryRequest.
/// </summary>
public sealed class CreateSkillCategoryRequestValidator
    : AbstractValidator<CreateSkillCategoryRequest>
{
    public CreateSkillCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name must not exceed 100 characters")
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
            .WithMessage("Color hex must be a valid hex color code (e.g., #FF5733 or #F73)");

        RuleFor(x => x.IconUrl)
            .Must(BeAValidUrl)
            .WithMessage("Icon URL must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.IconUrl));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}

/// <summary>
/// Validator for CreateSkillRequest.
/// </summary>
public sealed class CreateSkillRequestValidator : AbstractValidator<CreateSkillRequest>
{
    public CreateSkillRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Skill name is required")
            .MaximumLength(100)
            .WithMessage("Skill name must not exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-&.#+]+$")
            .WithMessage("Skill name contains invalid characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Skill description is required")
            .MaximumLength(1000)
            .WithMessage("Skill description must not exceed 1000 characters");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category ID is required")
            .NotEqual(Guid.Empty)
            .WithMessage("Category ID must be a valid GUID");
    }
}

/// <summary>
/// Validator for RegisterUserRequest.
/// </summary>
public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    private static readonly Regex TimeZoneRegex = new(
        @"^[A-Za-z_]+\/[A-Za-z_]+$",
        RegexOptions.Compiled
    );
    private static readonly Regex LanguageRegex = new(
        @"^[a-z]{2}(-[A-Z]{2})?$",
        RegexOptions.Compiled
    );

    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Email must not exceed 255 characters");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("First name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name must not exceed 50 characters")
            .Matches(@"^[a-zA-Z\s\-']+$")
            .WithMessage("Last name can only contain letters, spaces, hyphens, and apostrophes");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long")
            .MaximumLength(100)
            .WithMessage("Password must not exceed 100 characters")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]")
            .WithMessage(
                "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character"
            );

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Password confirmation is required")
            .Equal(x => x.Password)
            .WithMessage("Password confirmation must match the password");

        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .WithMessage("Timezone is required")
            .Must(BeAValidTimeZone)
            .WithMessage("Timezone must be a valid timezone identifier (e.g., America/New_York)");

        RuleFor(x => x.PreferredLanguage)
            .NotEmpty()
            .WithMessage("Preferred language is required")
            .Matches(LanguageRegex)
            .WithMessage("Preferred language must be a valid language code (e.g., en-US, fr-FR)");
    }

    private static bool BeAValidTimeZone(string timeZone)
    {
        if (string.IsNullOrEmpty(timeZone))
            return false;

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            // Also accept common timezone formats
            return TimeZoneRegex.IsMatch(timeZone);
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
}
