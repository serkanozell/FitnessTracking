using BuildingBlocks.Infrastructure.Persistence;
using Exercises.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Exercises.Infrastructure.Persistence
{
    public sealed class ExercisesDbContext(DbContextOptions<ExercisesDbContext> options)
        : ModuleDbContext(options)
    {
        public DbSet<Exercise> Exercises => Set<Exercise>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExercisesDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}