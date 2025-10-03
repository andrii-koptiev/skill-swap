using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the Role entity
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Table configuration
        builder.ToTable("roles");

        // Primary key
        builder.HasKey(r => r.Id);

        // Properties
        builder.Property(r => r.Id).HasColumnName("id").IsRequired();

        builder
            .Property(r => r.RoleType)
            .HasColumnName("role_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.Name).HasColumnName("name").HasMaxLength(100).IsRequired();

        builder.Property(r => r.Description).HasColumnName("description").HasMaxLength(500);

        builder
            .Property(r => r.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        // Audit properties
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at").IsRequired();

        // Indexes
        builder.HasIndex(r => r.RoleType).HasDatabaseName("ix_roles_role_type").IsUnique();

        builder.HasIndex(r => r.Name).HasDatabaseName("ix_roles_name").IsUnique();

        builder.HasIndex(r => r.IsActive).HasDatabaseName("ix_roles_is_active");

        // Relationships - Configure from the principal (Role) side
        builder
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
