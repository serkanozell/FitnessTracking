using System.Linq.Expressions;
using Nutrition.Domain.Entity;

namespace Nutrition.Domain.Repositories
{
    public interface IFoodRepository
    {
        Task<Food?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Food>> GetAllActiveAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Food>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<TResult> Items, int TotalCount)> GetPagedAsync<TResult>(Guid? userId, int pageNumber, int pageSize, Expression<Func<Food, TResult>> selector, CancellationToken cancellationToken = default);
        Task AddAsync(Food food, CancellationToken cancellationToken = default);
        void Update(Food food);
    }
}
