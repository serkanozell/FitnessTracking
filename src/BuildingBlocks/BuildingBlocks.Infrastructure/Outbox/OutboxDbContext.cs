using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxDbContext : DbContext
    {
        public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
            : base(options)
        {
        }

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}