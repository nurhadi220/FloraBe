using floral.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace floral.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(role => role.Id);

        builder.Property(role => role.Id)
            .HasColumnName("id")
            .HasColumnType("bigint")
            .ValueGeneratedOnAdd();

        builder.Property(role => role.Slug)
            .HasColumnName("slug")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(role => role.Slug)
            .IsUnique()
            .HasDatabaseName("uq_roles_slug");

        builder.Property(role => role.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(role => role.Description)
            .HasColumnName("description");

        builder.Property(role => role.IsSystem)
            .HasColumnName("is_system")
            .HasColumnType("tinyint(1)")
            .IsRequired();

        builder.Property(role => role.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(role => role.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired(false);

        builder.HasMany(role => role.UserRoles)
            .WithOne(userRole => userRole.Role)
            .HasForeignKey(userRole => userRole.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}