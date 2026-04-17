using Nutrition.Domain.Entity;

namespace Nutrition.Domain.Repositories
{
    public interface IMealPlanRepository
    {
        Task<MealPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MealPlan?> GetByIdWithMealsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MealPlan>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<MealPlan> Items, int TotalCount)> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);
        void Update(MealPlan mealPlan);
    }
}
