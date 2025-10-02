using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Infrastructure.Data.Seed;

/// <summary>
/// Seeds roles and permissions for the SkillSwap system
/// </summary>
public static class RoleSeed
{
    /// <summary>
    /// Seeds roles and their associated permissions
    /// </summary>
    /// <param name="context">The database context</param>
    public static void SeedRoles(SkillSwapDbContext context)
    {
        // Check if roles already exist
        if (context.Roles.Any())
        {
            return; // Roles already seeded
        }

        // Create system roles
        var roles = new List<Role>
        {
            new(
                RoleType.SuperAdmin,
                "Super Administrator",
                "Full system access with all permissions"
            ),
            new(
                RoleType.Administrator,
                "Administrator",
                "Administrative access to manage users and content"
            ),
            new(
                RoleType.Moderator,
                "Moderator",
                "Content moderation and user management permissions"
            ),
            new(RoleType.User, "User", "Standard user with basic platform permissions"),
        };

        // Add roles to context
        context.Roles.AddRange(roles);
        context.SaveChanges();

        // Now seed permissions for each role
        SeedRolePermissions(context, roles);
    }

    /// <summary>
    /// Seeds permissions for each role
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="roles">The roles to assign permissions to</param>
    private static void SeedRolePermissions(SkillSwapDbContext context, List<Role> roles)
    {
        var permissions = new List<RolePermission>();
        var systemSeeder = "System Seeder";

        // Get roles by type for easier assignment
        var superAdmin = roles.First(r => r.RoleType == RoleType.SuperAdmin);
        var admin = roles.First(r => r.RoleType == RoleType.Administrator);
        var moderator = roles.First(r => r.RoleType == RoleType.Moderator);
        var user = roles.First(r => r.RoleType == RoleType.User);

        // Super Admin - All permissions
        foreach (Permission permission in Enum.GetValues<Permission>())
        {
            permissions.Add(new RolePermission(superAdmin.Id, permission, true, systemSeeder));
        }

        // Administrator - Most permissions except super admin specific ones
        var adminPermissions = new[]
        {
            Permission.ViewUsers,
            Permission.CreateUsers,
            Permission.UpdateUsers,
            Permission.ViewSkills,
            Permission.CreateSkills,
            Permission.UpdateSkills,
            Permission.ModerateContent,
            Permission.ViewReports,
            Permission.ResolveReports,
            Permission.ViewSystemLogs,
            Permission.ManageRoles,
            Permission.ViewSwaps,
            Permission.CreateSwaps,
            Permission.UpdateSwaps,
            Permission.ViewProfiles,
            Permission.UpdateOwnProfile,
            Permission.UpdateAnyProfile,
        };

        foreach (var permission in adminPermissions)
        {
            permissions.Add(new RolePermission(admin.Id, permission, true, systemSeeder));
        }

        // Moderator - Content moderation and basic management
        var moderatorPermissions = new[]
        {
            Permission.ViewUsers,
            Permission.UpdateUsers,
            Permission.ViewSkills,
            Permission.CreateSkills,
            Permission.UpdateSkills,
            Permission.ModerateContent,
            Permission.ViewReports,
            Permission.ResolveReports,
            Permission.ViewSwaps,
            Permission.UpdateSwaps,
            Permission.ViewProfiles,
            Permission.UpdateOwnProfile,
            Permission.UpdateAnyProfile,
        };

        foreach (var permission in moderatorPermissions)
        {
            permissions.Add(new RolePermission(moderator.Id, permission, true, systemSeeder));
        }

        // User - Basic permissions
        var userPermissions = new[]
        {
            Permission.ViewSkills,
            Permission.CreateSkills,
            Permission.ViewSwaps,
            Permission.CreateSwaps,
            Permission.UpdateSwaps,
            Permission.ViewProfiles,
            Permission.UpdateOwnProfile,
        };

        foreach (var permission in userPermissions)
        {
            permissions.Add(new RolePermission(user.Id, permission, true, systemSeeder));
        }

        // Add all permissions to context
        context.RolePermissions.AddRange(permissions);
        context.SaveChanges();
    }
}
