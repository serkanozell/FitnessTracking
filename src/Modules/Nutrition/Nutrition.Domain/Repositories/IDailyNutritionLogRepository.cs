using Nutrition.Domain.Entity;

namespace Nutrition.Domain.Repositories
{
    public interface IDailyNutritionLogRepository
    {
        Task<DailyNutritionLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<DailyNutritionLog?> GetByUserAndDateAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<DailyNutritionLog> Items, int TotalCount)> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(DailyNutritionLog log, CancellationToken cancellationToken = default);
        void Update(DailyNutritionLog log);
    }
}
