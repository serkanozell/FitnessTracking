using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Infrastructure.Persistence
{
    public sealed class WorkoutProgramsDbContext(DbContextOptions<WorkoutProgramsDbContext> options)
        : ModuleDbContext(options)
    {
        public DbSet<WorkoutProgram> WorkoutPrograms => Set<WorkoutProgram>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutProgramsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}