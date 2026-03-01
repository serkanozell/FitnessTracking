using BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Infrastructure.Persistence
{
    public sealed class WorkoutSessionsDbContext : DbContext
    {
        public WorkoutSessionsDbContext(DbContextOptions<WorkoutSessionsDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sadece WorkoutSessions modülünün konfigürasyonlarını uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutSessionsDbContext).Assembly);

            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}