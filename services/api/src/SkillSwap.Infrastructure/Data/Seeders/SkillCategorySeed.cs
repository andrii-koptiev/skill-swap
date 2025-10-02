using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data;

namespace SkillSwap.Infrastructure.Data.Seeders;

/// <summary>
/// Seeder for skill categories
/// </summary>
public static class SkillCategorySeed
{
    /// <summary>
    /// Seeds the skill categories with predefined categories
    /// </summary>
    public static async Task SeedSkillCategoriesAsync(SkillSwapDbContext context)
    {
        if (await context.SkillCategories.AnyAsync())
            return;

        var skillCategories = new List<SkillCategory>
        {
            new("Technology", "Technology and programming skills", "technology", "#007ACC", "code"),
            new("Creative", "Creative and artistic skills", "creative", "#FF6B35", "palette"),
            new("Business", "Business and professional skills", "business", "#2E8B57", "briefcase"),
            new(
                "Language",
                "Language and communication skills",
                "language",
                "#8A2BE2",
                "message-circle"
            ),
            new("Health", "Health and fitness skills", "health", "#DC143C", "heart"),
            new("Culinary", "Culinary and cooking skills", "culinary", "#FF8C00", "chef-hat"),
            new("Crafts", "Crafts and hobby skills", "crafts", "#8B4513", "scissors"),
            new("Education", "Education and teaching skills", "education", "#4169E1", "book"),
            new("Music", "Music and musical skills", "music", "#9932CC", "music"),
            new("Sports", "Sports and recreation skills", "sports", "#228B22", "activity"),
            new("Science", "Science and research skills", "science", "#20B2AA", "microscope"),
            new(
                "Other",
                "Other skills not covered by above categories",
                "other",
                "#696969",
                "more-horizontal"
            ),
        };

        await context.SkillCategories.AddRangeAsync(skillCategories);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Seeded {skillCategories.Count} skill categories");
    }

    /// <summary>
    /// Gets skill category by name for seeding purposes
    /// </summary>
    public static async Task<SkillCategory?> GetCategoryByNameAsync(
        SkillSwapDbContext context,
        string categoryName
    )
    {
        return await context.SkillCategories.FirstOrDefaultAsync(sc => sc.Name == categoryName);
    }

    /// <summary>
    /// Gets skill category by slug for seeding purposes
    /// </summary>
    public static async Task<SkillCategory?> GetCategoryBySlugAsync(
        SkillSwapDbContext context,
        string slug
    )
    {
        return await context.SkillCategories.FirstOrDefaultAsync(sc => sc.Slug == slug);
    }
}
