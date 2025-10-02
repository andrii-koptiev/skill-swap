using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for SkillCategory entity
/// </summary>
public class SkillCategoryConfiguration : IEntityTypeConfiguration<SkillCategory>
{
    public void Configure(EntityTypeBuilder<SkillCategory> builder)
    {
        // Table configuration
        builder.ToTable("skill_categories");

        // Primary key
        builder.HasKey(sc => sc.Id);

        // Properties
        builder.Property(sc => sc.Name).IsRequired().HasMaxLength(50);

        builder.Property(sc => sc.Description).IsRequired().HasMaxLength(500);

        builder.Property(sc => sc.Slug).IsRequired().HasMaxLength(50);

        builder.Property(sc => sc.Color).HasMaxLength(7); // For hex color codes like #FF5733

        builder.Property(sc => sc.Icon).HasMaxLength(50);

        builder.Property(sc => sc.IsActive).IsRequired().HasDefaultValue(true);

        // Indexes
        builder.HasIndex(sc => sc.Name).IsUnique().HasDatabaseName("IX_SkillCategories_Name");

        builder.HasIndex(sc => sc.Slug).IsUnique().HasDatabaseName("IX_SkillCategories_Slug");

        builder.HasIndex(sc => sc.IsActive).HasDatabaseName("IX_SkillCategories_IsActive");

        // Relationships
        builder
            .HasMany(sc => sc.Skills)
            .WithOne(s => s.Category)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent category deletion if skills exist

        // Timestamp configuration (inherited from BaseEntity)
        builder.Property(sc => sc.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(sc => sc.UpdatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
