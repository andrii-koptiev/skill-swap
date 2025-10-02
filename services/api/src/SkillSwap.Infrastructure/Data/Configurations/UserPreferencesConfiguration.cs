using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for UserPreferences entity
/// </summary>
public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        // Table configuration
        builder.ToTable("user_preferences");

        // Primary key
        builder.HasKey(up => up.Id);
        builder.Property(up => up.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        // Foreign key (also serves as unique constraint)
        builder.Property(up => up.UserId).HasColumnName("user_id").IsRequired();

        // Notification preferences
        builder
            .Property(up => up.EmailNotifications)
            .HasColumnName("email_notifications")
            .HasDefaultValue(true);

        builder
            .Property(up => up.PushNotifications)
            .HasColumnName("push_notifications")
            .HasDefaultValue(true);

        builder
            .Property(up => up.SessionReminders)
            .HasColumnName("session_reminders")
            .HasDefaultValue(true);

        builder
            .Property(up => up.MarketingEmails)
            .HasColumnName("marketing_emails")
            .HasDefaultValue(false);

        // Session preferences
        builder
            .Property(up => up.PreferredSessionDuration)
            .HasColumnName("preferred_session_duration")
            .HasDefaultValue(60);

        builder.Property(up => up.MaxTravelDistance).HasColumnName("max_travel_distance");

        builder
            .Property(up => up.PreferredMeetingPlatform)
            .HasColumnName("preferred_meeting_platform")
            .HasMaxLength(50)
            .HasDefaultValue("built_in");

        builder
            .Property(up => up.AutoAcceptFromVerified)
            .HasColumnName("auto_accept_from_verified")
            .HasDefaultValue(false);

        // Base entity properties
        builder
            .Property(up => up.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(up => up.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Unique constraint - one preferences record per user
        builder.HasIndex(up => up.UserId).IsUnique().HasDatabaseName("uq_user_preferences_unique");

        // Relationships
        builder
            .HasOne(up => up.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreferences>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
