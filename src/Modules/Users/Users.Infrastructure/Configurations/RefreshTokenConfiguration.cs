using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Users.Domain.Entity;

namespace Users.Infrastructure.Configurations
{
    public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens", schema: "users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Token)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(x => x.Token)
                   .IsUnique();

            builder.Property(x => x.ExpiresOnUtc)
                   .IsRequired();

            builder.Property(x => x.CreatedOnUtc)
                   .IsRequired();

            builder.Property(x => x.RevokedOnUtc);
        }
    }
}