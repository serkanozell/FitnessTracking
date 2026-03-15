using Users.Domain.Entity;
using Users.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Users.Infrastructure.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable(name: "Roles", schema: "users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.HasIndex(x => x.Name)
                   .IsUnique();

            builder.Property(x => x.Description)
                   .HasMaxLength(500);

            // Audit
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.UpdatedDate);
            builder.Property(x => x.CreatedBy).HasMaxLength(100);
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.Property(x => x.RowVersion).IsRowVersion();

            // Seed default roles
            builder.HasData(
                new { Id = RoleConstants.AdminRoleId, Name = RoleConstants.Admin, Description = "Administrator role with full access", IsActive = true, IsDeleted = false },
                new { Id = RoleConstants.MemberRoleId, Name = RoleConstants.Member, Description = "Default member role", IsActive = true, IsDeleted = false }
            );
        }
    }
}