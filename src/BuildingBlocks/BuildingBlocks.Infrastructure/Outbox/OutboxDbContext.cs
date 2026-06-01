using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxDbContext(DbContextOptions<OutboxDbContext> options)
        : ModuleDbContext(options)
    {
        protected override string Schema => OutboxSchema.Name;

        protected override bool OwnsOutboxTable => true;
    }
}