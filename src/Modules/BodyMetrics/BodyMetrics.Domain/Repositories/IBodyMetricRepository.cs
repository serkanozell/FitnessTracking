using System.Linq.Expressions;
using BodyMetrics.Domain.Entity;

namespace BodyMetrics.Domain.Repositories
{
    public interface IBodyMetricRepository
    {
        Task<BodyMetric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<BodyMetric>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<TResult> Items, int TotalCount)> GetPagedByUserAsync<TResult>(Guid userId, int pageNumber, int pageSize, Expression<Func<BodyMetric, TResult>> selector, CancellationToken cancellationToken = default);
        Task AddAsync(BodyMetric bodyMetric, CancellationToken cancellationToken = default);
        void Update(BodyMetric bodyMetric);
    }
}