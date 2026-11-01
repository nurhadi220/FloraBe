using floral.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace floral.Infrastructure.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");

        builder.HasKey(userRole => new
        {
            userRole.UserId,
            userRole.RoleId
        });

        builder.Property(userRole => userRole.UserId)
            .HasColumnName("user_id")
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(userRole => userRole.RoleId)
            .HasColumnName("role_id")
            .HasColumnType("bigint")
            .IsRequired();

        builder.Property(userRole => userRole.AssignedBy)
            .HasColumnName("assigned_by")
            .HasColumnType("bigint")
            .IsRequired(false);

        builder.Property(userRole => userRole.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasOne(userRole => userRole.User)
            .WithMany()
            .HasForeignKey(userRole => userRole.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(userRole => userRole.Role)
            .WithMany(role => role.UserRoles)
            .HasForeignKey(userRole => userRole.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(userRole => userRole.AssignedByUser)
            .WithMany()
            .HasForeignKey(userRole => userRole.AssignedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(userRole => userRole.RoleId)
            .HasDatabaseName("ix_user_roles_role_id");

        builder.HasIndex(userRole => userRole.AssignedBy)
            .HasDatabaseName("fk_user_roles_assigned_by");
    }
}