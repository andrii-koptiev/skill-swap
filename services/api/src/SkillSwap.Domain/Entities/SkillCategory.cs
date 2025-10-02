using SkillSwap.Domain.Common;

namespace SkillSwap.Domain.Entities;

/// <summary>
/// Skill category entity representing different types of skills
/// </summary>
public class SkillCategory : BaseEntity
{
    /// <summary>
    /// Private constructor for Entity Framework
    /// </summary>
    private SkillCategory() { }

    /// <summary>
    /// Creates a new skill category
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="description">Category description</param>
    /// <param name="slug">URL-friendly slug</param>
    /// <param name="color">Optional color for UI representation</param>
    /// <param name="icon">Optional icon name for UI representation</param>
    public SkillCategory(
        string name,
        string description,
        string slug,
        string? color = null,
        string? icon = null
    )
    {
        Name = ValidateName(name);
        Description = description?.Trim() ?? string.Empty;
        Slug = ValidateSlug(slug);
        Color = color?.Trim();
        Icon = icon?.Trim();
        IsActive = true;
    }

    /// <summary>
    /// Category name (e.g., "Technology", "Arts", "Business")
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the category
    /// </summary>
    public string Slug { get; private set; } = string.Empty;

    /// <summary>
    /// Optional color code for UI representation (e.g., "#FF5733")
    /// </summary>
    public string? Color { get; private set; }

    /// <summary>
    /// Optional icon name for UI representation
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Whether the category is active and available for use
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Navigation property for skills in this category
    /// </summary>
    public virtual ICollection<Skill> Skills { get; private set; } = new List<Skill>();

    /// <summary>
    /// Updates the category information
    /// </summary>
    public void UpdateCategory(
        string name,
        string description,
        string? color = null,
        string? icon = null
    )
    {
        Name = ValidateName(name);
        Description = description?.Trim() ?? string.Empty;
        Color = color?.Trim();
        Icon = icon?.Trim();
        UpdateTimestamp();
    }

    /// <summary>
    /// Updates the category slug
    /// </summary>
    public void UpdateSlug(string slug)
    {
        Slug = ValidateSlug(slug);
        UpdateTimestamp();
    }

    /// <summary>
    /// Activates the category
    /// </summary>
    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    /// <summary>
    /// Deactivates the category
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    /// <summary>
    /// Validates the category name
    /// </summary>
    private static string ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        if (name.Length > 50)
            throw new ArgumentException("Category name cannot exceed 50 characters", nameof(name));

        return name.Trim();
    }

    /// <summary>
    /// Validates the category slug
    /// </summary>
    private static string ValidateSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
            throw new ArgumentException("Category slug cannot be empty", nameof(slug));

        if (slug.Length > 50)
            throw new ArgumentException("Category slug cannot exceed 50 characters", nameof(slug));

        // Ensure slug is URL-friendly
        if (!System.Text.RegularExpressions.Regex.IsMatch(slug, @"^[a-z0-9\-]+$"))
            throw new ArgumentException(
                "Category slug must contain only lowercase letters, numbers, and hyphens",
                nameof(slug)
            );

        return slug.Trim();
    }
}
