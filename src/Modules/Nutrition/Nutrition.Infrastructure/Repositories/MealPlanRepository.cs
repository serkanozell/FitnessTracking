using BuildingBlocks.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using Nutrition.Domain.Entity;
using Nutrition.Domain.Repositories;
using Nutrition.Infrastructure.Persistence;

namespace Nutrition.Infrastructure.Repositories
{
    public class MealPlanRepository(NutritionDbContext _context) : IMealPlanRepository
    {
        public async Task<MealPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MealPlans
                .Include(x => x.Meals)
                    .ThenInclude(x => x.MealItems)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<MealPlan?> GetByIdWithMealsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.MealPlans
                .Include(x => x.Meals)
                    .ThenInclude(x => x.MealItems)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<MealPlan>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.MealPlans
                .Include(x => x.Meals)
                    .ThenInclude(x => x.MealItems)
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<MealPlan> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.MealPlans
                .Include(x => x.Meals)
                    .ThenInclude(x => x.MealItems)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Date);

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task AddAsync(MealPlan mealPlan, CancellationToken cancellationToken = default)
            => await _context.MealPlans.AddAsync(mealPlan, cancellationToken);

        public void Update(MealPlan mealPlan)
            => _context.MealPlans.Update(mealPlan);
    }
}
