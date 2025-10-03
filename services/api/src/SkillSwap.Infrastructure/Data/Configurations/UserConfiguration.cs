using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkillSwap.Domain.Entities;
using SkillSwap.Domain.Enums;
using SkillSwap.Domain.ValueObjects;

namespace SkillSwap.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Table configuration
        builder.ToTable("users");

        // Primary key
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");

        // Email value object configuration
        builder
            .Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired()
            .HasConversion(email => email.Value, value => new Email(value));

        // Username configuration
        builder.Property(u => u.Username).HasColumnName("username").HasMaxLength(50).IsRequired();

        // Password hash
        builder
            .Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(255)
            .IsRequired();

        // User profile value object - owned type
        builder.OwnsOne(
            u => u.Profile,
            profile =>
            {
                profile
                    .Property(p => p.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(100)
                    .IsRequired();

                profile
                    .Property(p => p.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(100)
                    .IsRequired();

                profile.Property(p => p.Bio).HasColumnName("bio").HasMaxLength(1000);

                profile
                    .Property(p => p.ProfileImageUrl)
                    .HasColumnName("profile_image_url")
                    .HasMaxLength(500);

                profile.Property(p => p.Timezone).HasColumnName("timezone").HasMaxLength(50);

                profile
                    .Property(p => p.PreferredLanguage)
                    .HasColumnName("preferred_language")
                    .HasMaxLength(10)
                    .HasDefaultValue("en");
            }
        );

        // Enum configurations
        builder
            .Property(u => u.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(UserStatus.PendingVerification);

        builder
            .Property(u => u.VerificationStatus)
            .HasColumnName("verification_status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(VerificationStatus.Unverified);

        // Nullable datetime
        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");

        // Base entity properties
        builder
            .Property(u => u.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder
            .Property(u => u.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Unique constraints
        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("idx_users_email");

        builder.HasIndex(u => u.Username).IsUnique().HasDatabaseName("idx_users_username");

        // Performance indexes
        builder.HasIndex(u => u.Status).HasDatabaseName("idx_users_status");

        builder.HasIndex(u => u.VerificationStatus).HasDatabaseName("idx_users_verification");

        builder.HasIndex(u => u.LastLoginAt).HasDatabaseName("idx_users_last_login");

        // Relationships
        builder
            .HasMany(u => u.UserSkills)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.Availability)
            .WithOne(ua => ua.User)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(u => u.Preferences)
            .WithOne(up => up.User)
            .HasForeignKey<UserPreferences>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Navigation property configurations
        builder.Navigation(u => u.UserSkills).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(u => u.Availability).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
