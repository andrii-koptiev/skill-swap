using SkillSwap.Domain.Common;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Skill entity representing a specific skill that users can teach or learn
/// </summary>
public partial class Skill : BaseEntity
{
    // Private parameterless constructor for EF Core
    private Skill() { }

    public Skill(string name, string description, SkillCategory category)
    {
        Name = ValidateName(name);
        Slug = GenerateSlug(name);
        Description = description?.Trim();
        CategoryId = GetCategoryId(category);
        IsActive = true;
    }

    /// <summary>
    /// Skill name (e.g., "JavaScript", "Guitar Playing")
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// URL-friendly version of the name
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Detailed description of the skill
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Category ID for database storage
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Computed category property derived from CategoryId
    /// </summary>
    public SkillCategory Category => GetCategoryFromId(CategoryId);

    /// <summary>
    /// Whether the skill is active and available for use
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Updates the skill name and regenerates slug
    /// </summary>
    public void UpdateName(string name, string? description = null)
    {
        Name = ValidateName(name);
        Slug = GenerateSlug(name);
        Description = description?.Trim();
        UpdateTimestamp();
    }

    /// <summary>
    /// Changes the skill category
    /// </summary>
    public void ChangeCategory(SkillCategory category)
    {
        CategoryId = GetCategoryId(category);
        UpdateTimestamp();
    }

    /// <summary>
    /// Activates the skill
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    /// <summary>
    /// Deactivates the skill
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates and formats the skill name
    /// </summary>
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Skill name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Skill name cannot exceed 100 characters", nameof(name));

        return name.Trim();
    }

    private static string GenerateSlug(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        // Convert to lowercase and replace spaces/special chars with hyphens
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-")
            .Replace(".", "-")
            .Replace("#", "sharp")
            .Replace("+", "plus")
            .Replace("&", "and");

        // Remove any remaining non-alphanumeric characters except hyphens
        slug = MyRegex().Replace(slug, "");

        // Remove multiple consecutive hyphens
        slug = MyRegex1().Replace(slug, "-");

        // Remove leading/trailing hyphens
        slug = slug.Trim('-');

        return slug;
    }

    /// <summary>
    /// Converts SkillCategory enum to a fixed GUID for database storage
    /// </summary>
    private static Guid GetCategoryId(SkillCategory category)
    {
        // Create deterministic GUIDs for each category
        return category switch
        {
            SkillCategory.Technology => new Guid("11111111-1111-1111-1111-111111111111"),
            SkillCategory.Creative => new Guid("22222222-2222-2222-2222-222222222222"),
            SkillCategory.Business => new Guid("33333333-3333-3333-3333-333333333333"),
            SkillCategory.Language => new Guid("44444444-4444-4444-4444-444444444444"),
            SkillCategory.Health => new Guid("55555555-5555-5555-5555-555555555555"),
            SkillCategory.Culinary => new Guid("66666666-6666-6666-6666-666666666666"),
            SkillCategory.Crafts => new Guid("77777777-7777-7777-7777-777777777777"),
            SkillCategory.Education => new Guid("88888888-8888-8888-8888-888888888888"),
            SkillCategory.Music => new Guid("99999999-9999-9999-9999-999999999999"),
            SkillCategory.Sports => new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            SkillCategory.Science => new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            SkillCategory.Other => new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            _ => throw new ArgumentException(
                $"Unknown skill category: {category}",
                nameof(category)
            ),
        };
    }

    /// <summary>
    /// Converts GUID back to SkillCategory enum
    /// </summary>
    private static SkillCategory GetCategoryFromId(Guid categoryId)
    {
        return categoryId.ToString().ToLowerInvariant() switch
        {
            "11111111-1111-1111-1111-111111111111" => SkillCategory.Technology,
            "22222222-2222-2222-2222-222222222222" => SkillCategory.Creative,
            "33333333-3333-3333-3333-333333333333" => SkillCategory.Business,
            "44444444-4444-4444-4444-444444444444" => SkillCategory.Language,
            "55555555-5555-5555-5555-555555555555" => SkillCategory.Health,
            "66666666-6666-6666-6666-666666666666" => SkillCategory.Culinary,
            "77777777-7777-7777-7777-777777777777" => SkillCategory.Crafts,
            "88888888-8888-8888-8888-888888888888" => SkillCategory.Education,
            "99999999-9999-9999-9999-999999999999" => SkillCategory.Music,
            "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa" => SkillCategory.Sports,
            "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb" => SkillCategory.Science,
            "cccccccc-cccc-cccc-cccc-cccccccccccc" => SkillCategory.Other,
            _ => throw new ArgumentException(
                $"Unknown category ID: {categoryId}",
                nameof(categoryId)
            ),
        };
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();

    [System.Text.RegularExpressions.GeneratedRegex(@"-{2,}")]
    private static partial System.Text.RegularExpressions.Regex MyRegex1();
}
