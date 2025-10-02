using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the UserRole entity
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        // Table configuration
        builder.ToTable("user_roles");

        // Primary key
        builder.HasKey(ur => ur.Id);

        // Properties
        builder.Property(ur => ur.Id).HasColumnName("id").IsRequired();

        builder.Property(ur => ur.UserId).HasColumnName("user_id").IsRequired();

        builder.Property(ur => ur.RoleId).HasColumnName("role_id").IsRequired();

        builder.Property(ur => ur.AssignedAt).HasColumnName("assigned_at").IsRequired();

        builder
            .Property(ur => ur.AssignedBy)
            .HasColumnName("assigned_by")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ur => ur.ExpiresAt).HasColumnName("expires_at");

        builder
            .Property(ur => ur.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        // Audit properties
        builder.Property(ur => ur.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(ur => ur.UpdatedAt).HasColumnName("updated_at").IsRequired();

        // Indexes
        builder.HasIndex(ur => ur.UserId).HasDatabaseName("ix_user_roles_user_id");

        builder.HasIndex(ur => ur.RoleId).HasDatabaseName("ix_user_roles_role_id");

        builder
            .HasIndex(ur => new { ur.UserId, ur.RoleId })
            .HasDatabaseName("ix_user_roles_user_role")
            .IsUnique();

        builder.HasIndex(ur => ur.IsActive).HasDatabaseName("ix_user_roles_is_active");

        builder.HasIndex(ur => ur.ExpiresAt).HasDatabaseName("ix_user_roles_expires_at");

        // Relationships
        builder
            .HasOne(ur => ur.User)
            .WithMany()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ur => ur.Role)
            .WithMany()
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
