using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the UserSkill entity
/// </summary>
public class UserSkillConfiguration : IEntityTypeConfiguration<UserSkill>
{
    public void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        // Table configuration
        builder.ToTable("user_skills");

        // Primary key
        builder.HasKey(us => us.Id);

        // Properties
        builder.Property(us => us.Id).HasColumnName("id").IsRequired();

        builder.Property(us => us.UserId).HasColumnName("user_id").IsRequired();

        builder.Property(us => us.SkillId).HasColumnName("skill_id").IsRequired();

        builder
            .Property(us => us.SkillType)
            .HasColumnName("skill_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(us => us.ProficiencyLevel).HasColumnName("proficiency_level").IsRequired();

        builder.Property(us => us.YearsOfExperience).HasColumnName("years_of_experience");

        builder.Property(us => us.Description).HasColumnName("description").HasMaxLength(1000);

        builder
            .Property(us => us.IsPrimary)
            .HasColumnName("is_primary")
            .IsRequired()
            .HasDefaultValue(false);

        // Audit properties
        builder.Property(us => us.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(us => us.UpdatedAt).HasColumnName("updated_at").IsRequired();

        // Indexes
        builder.HasIndex(us => us.UserId).HasDatabaseName("ix_user_skills_user_id");

        builder.HasIndex(us => us.SkillId).HasDatabaseName("ix_user_skills_skill_id");

        builder
            .HasIndex(us => new
            {
                us.UserId,
                us.SkillId,
                us.SkillType,
            })
            .HasDatabaseName("ix_user_skills_user_skill_type")
            .IsUnique();

        builder.HasIndex(us => us.SkillType).HasDatabaseName("ix_user_skills_skill_type");

        builder
            .HasIndex(us => us.ProficiencyLevel)
            .HasDatabaseName("ix_user_skills_proficiency_level");

        builder.HasIndex(us => us.IsPrimary).HasDatabaseName("ix_user_skills_is_primary");

        // Relationships
        builder
            .HasOne(us => us.User)
            .WithMany(u => u.UserSkills)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(us => us.Skill)
            .WithMany()
            .HasForeignKey(us => us.SkillId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
