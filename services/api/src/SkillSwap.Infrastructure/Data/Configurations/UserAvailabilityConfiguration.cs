using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for UserAvailability entity
/// </summary>
public class UserAvailabilityConfiguration : IEntityTypeConfiguration<UserAvailability>
{
    public void Configure(EntityTypeBuilder<UserAvailability> builder)
    {
        // Table configuration
        builder.ToTable("user_availability");

        // Primary key
        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        // Foreign key
        builder.Property(ua => ua.UserId).HasColumnName("user_id").IsRequired();

        // Day of week (0-6, Sunday-Saturday)
        builder
            .Property(ua => ua.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<int>()
            .IsRequired();

        // Time slots
        builder
            .Property(ua => ua.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("time")
            .IsRequired();

        builder
            .Property(ua => ua.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("time")
            .IsRequired();

        // Timezone
        builder
            .Property(ua => ua.Timezone)
            .HasColumnName("timezone")
            .HasMaxLength(50)
            .IsRequired();

        // Is active
        builder.Property(ua => ua.IsActive).HasColumnName("is_active").HasDefaultValue(true);

        // Base entity properties
        builder
            .Property(ua => ua.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(ua => ua.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Performance indexes
        builder.HasIndex(ua => ua.UserId).HasDatabaseName("idx_user_availability_user");

        builder.HasIndex(ua => ua.DayOfWeek).HasDatabaseName("idx_user_availability_day");

        builder.HasIndex(ua => ua.IsActive).HasDatabaseName("idx_user_availability_active");

        // Composite index for efficient availability queries
        builder
            .HasIndex(ua => new { ua.DayOfWeek, ua.IsActive })
            .HasDatabaseName("idx_user_availability_day_active");

        // Relationships
        builder
            .HasOne(ua => ua.User)
            .WithMany(u => u.Availability)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Check constraint for time validation (PostgreSQL)
        builder.ToTable(tb =>
            tb.HasCheckConstraint("chk_availability_time", "start_time < end_time")
        );
    }
}
