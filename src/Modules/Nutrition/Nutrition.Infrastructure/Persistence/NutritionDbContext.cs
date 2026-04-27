using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entity;

namespace Nutrition.Infrastructure.Persistence
{
    public sealed class NutritionDbContext(DbContextOptions<NutritionDbContext> options)
        : ModuleDbContext(options)
    {
        public DbSet<Food> Foods => Set<Food>();
        public DbSet<MealPlan> MealPlans => Set<MealPlan>();
        public DbSet<DailyNutritionLog> DailyNutritionLogs => Set<DailyNutritionLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutritionDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
