using BuildingBlocks.Infrastructure.Outbox;
using Exercises.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Infrastructure.Persistence
{
    public sealed class ExercisesDbContext : DbContext
    {
        public ExercisesDbContext(DbContextOptions<ExercisesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sadece Exercises modülünün konfigürasyonlarını uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExercisesDbContext).Assembly);

            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}