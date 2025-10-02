using Microsoft.EntityFrameworkCore;
using SkillSwap.Domain.Common;
using SkillSwap.Domain.Entities;
using SkillSwap.Infrastructure.Data.Configurations;

namespace SkillSwap.Infrastructure.Data;

/// <summary>
/// Main application database context
/// </summary>
public class SkillSwapDbContext : DbContext
{
    public SkillSwapDbContext(DbContextOptions<SkillSwapDbContext> options)
        : base(options) { }

    // User-related entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserSkill> UserSkills { get; set; } = null!;
    public DbSet<UserAvailability> UserAvailability { get; set; } = null!;
    public DbSet<UserPreferences> UserPreferences { get; set; } = null!;

    // Role-related entities
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserRole> UserRoles { get; set; } = null!;
    public DbSet<RolePermission> RolePermissions { get; set; } = null!;

    // Skill-related entities
    public DbSet<Skill> Skills { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserSkillConfiguration());
        modelBuilder.ApplyConfiguration(new UserAvailabilityConfiguration());
        modelBuilder.ApplyConfiguration(new UserPreferencesConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());

        // Global configurations
        ConfigureGlobalSettings(modelBuilder);
    }

    private static void ConfigureGlobalSettings(ModelBuilder modelBuilder)
    {
        // Configure all DateTime properties to be stored as UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<
                            DateTime,
                            DateTime
                        >(v => v.ToUniversalTime(), v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                    );
                }
            }
        }

        // Configure PostgreSQL-specific settings
        ConfigurePostgreSqlSettings(modelBuilder);
    }

    private static void ConfigurePostgreSqlSettings(ModelBuilder modelBuilder)
    {
        // PostgreSQL-specific configurations can be added here when needed
        // Extensions are typically enabled through migrations
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically update UpdatedAt timestamps for modified entities
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e =>
                e.Entity is BaseEntity
                && (e.State == EntityState.Added || e.State == EntityState.Modified)
            );

        foreach (var entityEntry in entries)
        {
            if (entityEntry.Entity is BaseEntity entity)
            {
                entity.UpdateTimestamp();
            }
        }
    }
}
