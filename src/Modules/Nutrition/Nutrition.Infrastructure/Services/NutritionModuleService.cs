using Microsoft.EntityFrameworkCore;
using Nutrition.Contracts;
using Nutrition.Infrastructure.Persistence;

namespace Nutrition.Infrastructure.Services
{
    internal sealed class NutritionModuleService(NutritionDbContext _context) : INutritionModule
    {
        public async Task<DailyNutritionSummary?> GetDailySummaryAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            // Aggregate macro totals in SQL instead of loading the full
            // MealPlan -> Meals -> MealItems graph into memory. Nullable casts guard
            // against Sum over an empty owned-collection subquery.
            var summary = await _context.MealPlans
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Date.Date == date.Date && !x.IsDeleted)
                .Select(x => new DailyNutritionSummary(
                    x.Date,
                    x.Meals.SelectMany(m => m.MealItems).Sum(i => (decimal?)i.Macros.Calories) ?? 0m,
                    x.Meals.SelectMany(m => m.MealItems).Sum(i => (decimal?)i.Macros.Protein) ?? 0m,
                    x.Meals.SelectMany(m => m.MealItems).Sum(i => (decimal?)i.Macros.Carbohydrates) ?? 0m,
                    x.Meals.SelectMany(m => m.MealItems).Sum(i => (decimal?)i.Macros.Fat) ?? 0m,
                    x.Meals.Count))
                .FirstOrDefaultAsync(cancellationToken);

            return summary;
        }
    }
}
