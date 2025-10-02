using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data.Seeders;

namespace SkillSwap.Infrastructure.Data.Seed;

public static class SkillSeed
{
    public static async Task SeedSkillsAsync(SkillSwapDbContext context)
    {
        if (await context.Skills.AnyAsync())
        {
            return; // Skills already seeded
        }

        // Ensure skill categories are seeded first
        await SkillCategorySeed.SeedSkillCategoriesAsync(context);

        // Get category references
        var categories = await context.SkillCategories.ToListAsync();
        var categoryMap = categories.ToDictionary(c => c.Name, c => c.Id);

        var skills = new List<Skill>();

        // Technology Skills
        var techCategoryId = categoryMap["Technology"];
        skills.AddRange(
            new[]
            {
                new Skill(
                    "JavaScript",
                    "Modern JavaScript programming including ES6+ features",
                    techCategoryId
                ),
                new Skill(
                    "Python",
                    "Python programming for web development and data science",
                    techCategoryId
                ),
                new Skill(
                    "React",
                    "Building interactive user interfaces with React",
                    techCategoryId
                ),
                new Skill("Node.js", "Server-side JavaScript development", techCategoryId),
                new Skill("C#", "C# programming for .NET applications", techCategoryId),
            }
        );

        // Creative Skills
        var creativeCategoryId = categoryMap["Creative"];
        skills.AddRange(
            new[]
            {
                new Skill("Digital Art", "Digital illustration and design", creativeCategoryId),
                new Skill("Photography", "Portrait and landscape photography", creativeCategoryId),
                new Skill("Video Editing", "Video production and editing", creativeCategoryId),
                new Skill("Graphic Design", "Logo and visual design", creativeCategoryId),
                new Skill("3D Modeling", "3D modeling and animation", creativeCategoryId),
            }
        );

        // Business Skills
        var businessCategoryId = categoryMap["Business"];
        skills.AddRange(
            new[]
            {
                new Skill(
                    "Project Management",
                    "Agile and Scrum project management",
                    businessCategoryId
                ),
                new Skill(
                    "Digital Marketing",
                    "SEO and social media marketing",
                    businessCategoryId
                ),
                new Skill("Sales", "Lead generation and negotiation", businessCategoryId),
                new Skill(
                    "Public Speaking",
                    "Presentation and communication skills",
                    businessCategoryId
                ),
                new Skill("Leadership", "Team management and leadership", businessCategoryId),
            }
        );

        // Add skills to context
        context.Skills.AddRange(skills);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Seeded {skills.Count} skills across {categories.Count} categories");
    }
}
