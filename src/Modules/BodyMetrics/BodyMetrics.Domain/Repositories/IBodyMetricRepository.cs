using BodyMetrics.Domain.Entity;

namespace BodyMetrics.Domain.Repositories
{
    public interface IBodyMetricRepository
    {
        Task<BodyMetric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<BodyMetric> Items, int TotalCount)> GetPagedByUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task AddAsync(BodyMetric bodyMetric, CancellationToken cancellationToken = default);
        void Update(BodyMetric bodyMetric);
    }
}