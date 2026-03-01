using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages", "outbox");

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

            builder.HasIndex(x => new { x.IsProcessed, x.OccurredOnUtc })
                   .HasFilter("[IsProcessed] = 0");
        }
    }
}