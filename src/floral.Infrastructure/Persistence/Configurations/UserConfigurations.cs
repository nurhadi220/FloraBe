using floral.Domain.Entities;
using floral.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace floral.Infrastructure.Persintence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Nama tabel
        builder.ToTable("users");

        // Primary Key
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();

        // Public ID
        builder.Property(user => user.PublicId)
            .HasColumnName("public_id")
            .HasColumnType("varchar(255)")
            .IsRequired();

        // Name
        builder.Property(user => user.Name)
            .HasColumnName("name")
            .HasColumnType("varchar(120)")
            .IsRequired();

        // Email
        builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasColumnType("varchar(255)")
            .IsRequired(false);

        // Gender
        builder.Property(user => user.Gender)
        .HasColumnName("jenis_kelamin")
        .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<GenderUser>(
                    value,
                    ignoreCase: true))
            .IsRequired();

        // Username
        builder.Property(user => user.Username)
            .HasColumnName("username")
            .HasColumnType("varchar(30)")
            .IsRequired();

        builder.HasIndex(user => user.Username)
            .IsUnique()
            .HasDatabaseName("ux_users_username");

        // Phone Number
        builder.Property(user => user.PhoneNumber)
            .HasColumnName("phone_number")
            .HasColumnType("varchar(20)")
            .IsRequired(false);

        // Email Verification
        builder.Property(user => user.EmailVerifiedAt)
            .HasColumnName("email_verified_at")
            .HasColumnType("timestamp")
            .IsRequired(false);

        // Phone Verification
        builder.Property(user => user.PhoneVerifiedAt)
            .HasColumnName("phone_verified_at")
            .HasColumnType("timestamp")
            .IsRequired(false);

        // Password
        builder.Property(user => user.Password)
            .HasColumnName("password")
            .HasColumnType("varchar(255)")
            .IsRequired(false);

        // Avatar Path
        builder.Property(user => user.AvatarPath)
            .HasColumnName("avatar_path")
            .HasColumnType("varchar(255)")
            .IsRequired(false);

        // Account Status
        builder.Property(user => user.Status)
            .HasColumnName("status")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<UserStatus>(
                    value,
                    ignoreCase: true))
            .IsRequired();

        // Registration Source
        builder.Property(user => user.RegistrationSource)
            .HasColumnName("registration_source")
            .HasConversion(
                value => value.ToString().ToLowerInvariant(),
                value => Enum.Parse<RegistrationSource>(
                    value,
                    ignoreCase: true))
            .IsRequired();

        // Terms Accepted At
        builder.Property(user => user.TermsAcceptedAt)
            .HasColumnName("terms_accepted_at")
            .HasColumnType("varchar(20)")
            .HasConversion(
                value => value.ToString("yyyy-MM-dd HH:mm:ss"),
                value => DateTime.Parse(value))
            .IsRequired();

        // Terms Version
        builder.Property(user => user.TermsVersion)
            .HasColumnName("terms_version")
            .HasColumnType("varchar(10)")
            .IsRequired();

        // Marketing Opt In
        builder.Property(user => user.MarketingOptInAt)
            .HasColumnName("marketing_opt_in_at")
            .HasColumnType("timestamp")
            .IsRequired(false);

        // Last Login
        builder.Property(user => user.LastLoginAt)
            .HasColumnName("last_login_at")
            .HasColumnType("timestamp")
            .IsRequired(false);

        // Remember Token
        builder.Property(user => user.RememberToken)
            .HasColumnName("remember_token")
            .HasColumnType("varchar(100)")
            .IsRequired(false);

        // Created At
        builder.Property(user => user.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp")
            .IsRequired(false);

        // Updated At
        builder.Property(user => user.UpdatedAt)
            .HasColumnName("updated_at")
            .HasColumnType("timestamp")
            .IsRequired(false);

        // Deleted At
        builder.Property(user => user.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp")
            .IsRequired(false);
    }
}
