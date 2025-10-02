using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;
using SkillSwap.Domain.ValueObjects;
using SkillSwap.Infrastructure.Data.Seed;

namespace SkillSwap.Infrastructure.Data.Extensions;

/// <summary>
/// Database context seeding extensions for development environment
/// </summary>
public static class ContextSeederExtensions
{
    /// <summary>
    /// Seeds the database with development data
    /// </summary>
    public static async Task SeedDevelopmentDataAsync(
        this IServiceProvider serviceProvider,
        ILogger logger
    )
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SkillSwapDbContext>();
        var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

        // Only seed in development environment
        if (!environment.IsDevelopment())
        {
            logger.LogInformation("Skipping data seeding - not in development environment");
            return;
        }

        try
        {
            logger.LogInformation("Starting database seeding for development environment...");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed system data (roles and permissions)
            await SeedSystemDataAsync(context, logger);

            // Seed skills data
            await SeedSkillsDataAsync(context, logger);

            // Seed development data (test users)
            await SeedDevelopmentUsersAsync(context, logger);

            // Assign roles to users (always run to ensure roles are assigned)
            await AssignRolesToUsersAsync(context, logger);

            // Seed user skills associations
            await SeedUserSkillsAsync(context, logger);

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while seeding the database. Seeding failed and will need to be retried"
            );
            throw new InvalidOperationException(
                "Database seeding failed during development data initialization",
                ex
            );
        }
    }

    /// <summary>
    /// Seeds system data that should exist in all environments
    /// </summary>
    private static async Task SeedSystemDataAsync(SkillSwapDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding system roles and permissions...");

        // Seed roles and permissions
        RoleSeed.SeedRoles(context);

        await context.SaveChangesAsync();
        logger.LogInformation("System roles and permissions seeded successfully");
    }

    /// <summary>
    /// Seeds development test data
    /// </summary>
    private static async Task SeedDevelopmentUsersAsync(SkillSwapDbContext context, ILogger logger)
    {
        // Check if users already exist
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Users already exist, skipping user seeding");
            return;
        }

        logger.LogInformation("Seeding development users...");

        var users = new List<User>();

        // Create test users
        users.Add(
            CreateTestUser(
                "superadmin@skillswap.dev",
                "superadmin",
                "Super",
                "Admin",
                "System super administrator for testing"
            )
        );

        users.Add(
            CreateTestUser(
                "admin@skillswap.dev",
                "admin",
                "John",
                "Administrator",
                "Platform administrator for testing"
            )
        );

        users.Add(
            CreateTestUser(
                "moderator@skillswap.dev",
                "moderator",
                "Jane",
                "Moderator",
                "Community moderator for testing"
            )
        );

        users.Add(
            CreateTestUser(
                "alice@skillswap.dev",
                "alice_teacher",
                "Alice",
                "Johnson",
                "Language teacher from Spain. I love teaching Spanish and want to learn programming!"
            )
        );

        users.Add(
            CreateTestUser(
                "bob@skillswap.dev",
                "bob_dev",
                "Bob",
                "Smith",
                "Full-stack developer from USA. I can teach React and Python, looking to learn Japanese."
            )
        );

        users.Add(
            CreateTestUser(
                "carol@skillswap.dev",
                "carol_music",
                "Carol",
                "Davis",
                "Professional pianist from Germany. I teach piano and music theory, want to learn photography."
            )
        );

        users.Add(
            CreateTestUser(
                "david@skillswap.dev",
                "david_chef",
                "David",
                "Wilson",
                "Professional chef from France. I can teach French cooking, interested in learning guitar."
            )
        );

        users.Add(
            CreateTestUser(
                "emma@skillswap.dev",
                "emma_designer",
                "Emma",
                "Brown",
                "UI/UX designer from Canada. I teach design principles, want to learn Italian language."
            )
        );

        // Add users to context
        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {UserCount} development users", users.Count);
    }

    /// <summary>
    /// Creates a test user with specified details
    /// </summary>
    private static User CreateTestUser(
        string email,
        string username,
        string firstName,
        string lastName,
        string bio
    )
    {
        var userEmail = new Email(email);
        var userProfile = new UserProfile(firstName, lastName, bio, timezone: "UTC");

        // Use a simple hash for development - in real app, use proper password hashing
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!");

        var user = new User(userEmail, username, passwordHash, userProfile);

        // Activate and verify development users
        user.Activate();
        user.VerifyEmail();

        return user;
    }

    /// <summary>
    /// Assigns appropriate roles to seeded users
    /// </summary>
    private static async Task AssignRolesToUsersAsync(SkillSwapDbContext context, ILogger logger)
    {
        logger.LogInformation("Assigning roles to development users...");

        // Check if user roles are already assigned
        if (await context.Set<UserRole>().AnyAsync())
        {
            logger.LogInformation("User roles already exist, skipping role assignment");
            return;
        }

        // Clear tracking to avoid conflicts
        context.ChangeTracker.Clear();

        // Get fresh user and role data from database
        var dbUsers = await context.Users.ToListAsync();
        var dbRoles = await context.Roles.ToListAsync();

        var roleAssignments = new List<(string email, RoleType roleType)>
        {
            ("superadmin@skillswap.dev", RoleType.SuperAdmin),
            ("admin@skillswap.dev", RoleType.Administrator),
            ("moderator@skillswap.dev", RoleType.Moderator),
            ("alice@skillswap.dev", RoleType.User),
            ("bob@skillswap.dev", RoleType.User),
            ("carol@skillswap.dev", RoleType.User),
            ("david@skillswap.dev", RoleType.User),
            ("emma@skillswap.dev", RoleType.User),
        };

        // Create UserRole entities directly to avoid EF tracking issues
        var userRolesToAdd = new List<UserRole>();

        foreach (var (email, roleType) in roleAssignments)
        {
            var user = dbUsers.FirstOrDefault(u => u.Email.Value == email);
            var role = dbRoles.FirstOrDefault(r => r.RoleType == roleType);

            if (user != null && role != null)
            {
                var userRole = new UserRole(user.Id, role.Id, assignedBy: "System Seeder");
                userRolesToAdd.Add(userRole);
                logger.LogDebug(
                    "Created role assignment {RoleType} for user {Email}",
                    roleType,
                    email
                );
            }
        }

        // Add all user roles in batch
        context.Set<UserRole>().AddRange(userRolesToAdd);
        await context.SaveChangesAsync();

        logger.LogInformation(
            "Role assignments completed - assigned {Count} roles",
            userRolesToAdd.Count
        );
    }

    /// <summary>
    /// Seeds skills data for the platform
    /// </summary>
    private static async Task SeedSkillsDataAsync(SkillSwapDbContext context, ILogger logger)
    {
        logger.LogInformation("Seeding skills data...");

        // Seed skills
        await SkillSeed.SeedSkillsAsync(context);

        await context.SaveChangesAsync();
        logger.LogInformation("Skills seeded successfully");
    }

    /// <summary>
    /// Seeds user-skill associations for development users
    /// </summary>
    private static async Task SeedUserSkillsAsync(SkillSwapDbContext context, ILogger logger)
    {
        // Check if user skills already exist
        if (await context.UserSkills.AnyAsync())
        {
            logger.LogInformation("User skills already exist, skipping user skills seeding");
            return;
        }

        logger.LogInformation("Seeding user skills associations...");

        // Get users and some skills for associations
        var users = await context.Users.ToListAsync();
        var skills = await context.Skills.Take(8).ToListAsync(); // Get first 8 skills

        if (!users.Any() || !skills.Any())
        {
            logger.LogWarning("Cannot seed user skills - no users or skills found");
            return;
        }

        var userSkills = new List<UserSkill>();

        // Create user-skill associations based on usernames
        foreach (var user in users)
        {
            // Find a skill based on username or assign sequentially
            var skillIndex = users.IndexOf(user) % skills.Count;
            var skill = skills[skillIndex];

            // Each user can teach one skill with medium-high proficiency
            var userSkill = new UserSkill(
                userId: user.Id,
                skillId: skill.Id,
                skillType: SkillType.CanTeach,
                proficiencyLevel: 4, // High proficiency
                yearsOfExperience: 3, // 3 years experience
                description: $"Experienced in {skill.Name.ToLower()} with practical hands-on experience",
                isPrimary: true
            );

            userSkills.Add(userSkill);
        }

        if (userSkills.Any())
        {
            context.UserSkills.AddRange(userSkills);
            await context.SaveChangesAsync();
            logger.LogInformation(
                "User skills seeded successfully - created {Count} user-skill associations",
                userSkills.Count
            );
        }
    }
}
