using Microsoft.EntityFrameworkCore;
using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Infrastructure.Persistence
{
    public sealed class WorkoutProgramsDbContext : DbContext
    {
        public WorkoutProgramsDbContext(DbContextOptions<WorkoutProgramsDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkoutProgram> WorkoutPrograms => Set<WorkoutProgram>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Sadece WorkoutPrograms modülünün konfigürasyonlarını uygula
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkoutProgramsDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}