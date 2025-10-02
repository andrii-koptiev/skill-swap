using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Skill entity
/// </summary>
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        // Table configuration
        builder.ToTable("skills");

        // Primary key
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        // Name configuration
        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        // Slug configuration
        builder.Property(s => s.Slug).HasColumnName("slug").HasMaxLength(100).IsRequired();

        // Description configuration
        builder.Property(s => s.Description).HasColumnName("description").HasMaxLength(500);

        // Category ID configuration (foreign key to skill_categories)
        builder.Property(s => s.CategoryId).HasColumnName("category_id").IsRequired();

        // Configure foreign key relationship
        builder
            .HasOne(s => s.Category)
            .WithMany(sc => sc.Skills)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // IsActive configuration
        builder.Property(s => s.IsActive).HasColumnName("is_active").HasDefaultValue(true);

        // Audit fields from BaseEntity
        builder
            .Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Indexes
        builder.HasIndex(s => s.Slug).HasDatabaseName("idx_skills_slug").IsUnique();

        builder.HasIndex(s => s.Name).HasDatabaseName("idx_skills_name");

        builder.HasIndex(s => s.CategoryId).HasDatabaseName("idx_skills_category");

        builder.HasIndex(s => s.IsActive).HasDatabaseName("idx_skills_is_active");
    }
}
