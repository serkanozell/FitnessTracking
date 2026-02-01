using Microsoft.EntityFrameworkCore;
using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Infrastructure.Persistance
{
    public sealed class WorkoutSessionsDbContext : DbContext
    {
        public WorkoutSessionsDbContext(DbContextOptions<WorkoutSessionsDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sadece WorkoutSessions modülünün konfigürasyonlarını uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutSessionsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}