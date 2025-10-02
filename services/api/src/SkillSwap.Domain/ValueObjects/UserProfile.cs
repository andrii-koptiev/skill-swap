namespace SkillSwap.Domain.ValueObjects;

/// <summary>
/// Value object for user profile information
/// </summary>
public record UserProfile
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? Bio { get; init; }
    public string? ProfileImageUrl { get; init; }
    public string? Timezone { get; init; }
    public string PreferredLanguage { get; init; }

    public UserProfile(
        string firstName,
        string lastName,
        string? bio = null,
        string? profileImageUrl = null,
        string? timezone = null,
        string preferredLanguage = "en"
    )
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty", nameof(lastName));

        if (firstName.Length > 100)
            throw new ArgumentException(
                "First name cannot exceed 100 characters",
                nameof(firstName)
            );

        if (lastName.Length > 100)
            throw new ArgumentException("Last name cannot exceed 100 characters", nameof(lastName));

        if (!string.IsNullOrEmpty(bio) && bio.Length > 1000)
            throw new ArgumentException("Bio cannot exceed 1000 characters", nameof(bio));

        if (!string.IsNullOrEmpty(profileImageUrl) && profileImageUrl.Length > 500)
            throw new ArgumentException(
                "Profile image URL cannot exceed 500 characters",
                nameof(profileImageUrl)
            );

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Bio = bio?.Trim();
        ProfileImageUrl = profileImageUrl?.Trim();
        Timezone = timezone?.Trim();
        PreferredLanguage = preferredLanguage.Trim().ToLowerInvariant();
    }

    public string FullName => $"{FirstName} {LastName}";

    public UserProfile UpdateBio(string? bio) => this with { Bio = bio };

    public UserProfile UpdateProfileImage(string? profileImageUrl) =>
        this with
        {
            ProfileImageUrl = profileImageUrl,
        };

    public UserProfile UpdateTimezone(string? timezone) => this with { Timezone = timezone };

    public UserProfile UpdatePreferredLanguage(string preferredLanguage) =>
        this with
        {
            PreferredLanguage = preferredLanguage,
        };
}
