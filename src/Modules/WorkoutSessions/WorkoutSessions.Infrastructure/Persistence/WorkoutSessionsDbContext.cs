using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Infrastructure.Persistence
{
    public sealed class WorkoutSessionsDbContext(DbContextOptions<WorkoutSessionsDbContext> options)
        : ModuleDbContext(options)
    {
        public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutSessionsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}