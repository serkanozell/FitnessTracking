using Nutrition.Domain.Entity;

namespace Nutrition.Domain.Repositories
{
    public interface IFoodRepository
    {
        Task<Food?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Food>> GetAllActiveAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Food>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Food> Items, int TotalCount)> GetPagedAsync(Guid? userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(Food food, CancellationToken cancellationToken = default);
        void Update(Food food);
    }
}
