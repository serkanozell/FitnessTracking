using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxMessageConfiguration(bool excludeFromMigrations)
        : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            if (excludeFromMigrations)
            {
                builder.ToTable("OutboxMessages", OutboxSchema.Name, t => t.ExcludeFromMigrations());
            }
            else
            {
                builder.ToTable("OutboxMessages", OutboxSchema.Name);
            }

            builder.HasKey(x => x.Id);

            builder.Property(x => x.EventType)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(x => x.Content)
                   .IsRequired();

            builder.Property(x => x.IsProcessed)
                   .IsRequired();

            builder.Property(x => x.OccurredOnUtc)
                   .IsRequired();

            builder.Property(x => x.RetryCount)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.HasIndex(x => new { x.IsProcessed, x.OccurredOnUtc })
                   .HasFilter("[IsProcessed] = 0");
        }
    }
}