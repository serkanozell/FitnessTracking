using Microsoft.EntityFrameworkCore;
using Nutrition.Contracts;
using Nutrition.Infrastructure.Persistence;

namespace Nutrition.Infrastructure.Services
{
    internal sealed class NutritionModuleService(NutritionDbContext _context) : INutritionModule
    {
        public async Task<DailyNutritionSummary?> GetDailySummaryAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            var mealPlan = await _context.MealPlans
                .Include(x => x.Meals)
                    .ThenInclude(x => x.MealItems)
                .Where(x => x.UserId == userId && x.Date.Date == date.Date && !x.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);

            if (mealPlan is null) return null;

            var allItems = mealPlan.Meals.SelectMany(m => m.MealItems).ToList();

            return new DailyNutritionSummary(
                mealPlan.Date,
                allItems.Sum(i => i.Macros.Calories),
                allItems.Sum(i => i.Macros.Protein),
                allItems.Sum(i => i.Macros.Carbohydrates),
                allItems.Sum(i => i.Macros.Fat),
                mealPlan.Meals.Count);
        }
    }
}
