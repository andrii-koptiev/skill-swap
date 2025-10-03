using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for the RolePermission entity
/// </summary>
public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        // Table configuration
        builder.ToTable("role_permissions");

        // Primary key
        builder.HasKey(rp => rp.Id);

        // Properties
        builder.Property(rp => rp.Id).HasColumnName("id").IsRequired();

        builder.Property(rp => rp.RoleId).HasColumnName("role_id").IsRequired();

        builder
            .Property(rp => rp.Permission)
            .HasColumnName("permission")
            .HasConversion<int>()
            .IsRequired();

        builder
            .Property(rp => rp.IsGranted)
            .HasColumnName("is_granted")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(rp => rp.GrantedAt).HasColumnName("granted_at").IsRequired();

        builder
            .Property(rp => rp.GrantedBy)
            .HasColumnName("granted_by")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(rp => rp.Conditions).HasColumnName("conditions").HasMaxLength(1000);

        // Audit properties
        builder.Property(rp => rp.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.Property(rp => rp.UpdatedAt).HasColumnName("updated_at").IsRequired();

        // Indexes
        builder.HasIndex(rp => rp.RoleId).HasDatabaseName("ix_role_permissions_role_id");

        builder.HasIndex(rp => rp.Permission).HasDatabaseName("ix_role_permissions_permission");

        builder
            .HasIndex(rp => new { rp.RoleId, rp.Permission })
            .HasDatabaseName("ix_role_permissions_role_permission")
            .IsUnique();

        builder.HasIndex(rp => rp.IsGranted).HasDatabaseName("ix_role_permissions_is_granted");

        // Relationships - Role relationship configured in RoleConfiguration
        // No additional relationship configuration needed here
    }
}
