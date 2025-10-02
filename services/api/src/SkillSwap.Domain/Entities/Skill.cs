using SkillSwap.Domain.Common;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Skill entity representing a specific skill that users can teach or learn
/// </summary>
public partial class Skill : BaseEntity
{
    // Private parameterless constructor for EF Core
    private Skill() { }

    public Skill(string name, string description, Guid categoryId)
    {
        Name = ValidateName(name);
        Slug = GenerateSlug(name);
        Description = description?.Trim();
        CategoryId = categoryId;
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
    /// Category ID for foreign key relationship
    /// </summary>
    public Guid CategoryId { get; private set; }

    /// <summary>
    /// Navigation property to the skill category
    /// </summary>
    public virtual SkillCategory Category { get; private set; } = null!;

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
    public void ChangeCategory(Guid categoryId)
    {
        CategoryId = categoryId;
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

    [System.Text.RegularExpressions.GeneratedRegex(@"[^a-z0-9\-]")]
    private static partial System.Text.RegularExpressions.Regex MyRegex();

    [System.Text.RegularExpressions.GeneratedRegex(@"-{2,}")]
    private static partial System.Text.RegularExpressions.Regex MyRegex1();
}
