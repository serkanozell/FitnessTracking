using BuildingBlocks.Infrastructure.Pagination;
using BodyMetrics.Domain.Entity;
using BodyMetrics.Domain.Repositories;
using BodyMetrics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BodyMetrics.Infrastructure.Repositories
{
    public class BodyMetricRepository(BodyMetricsDbContext _context) : IBodyMetricRepository
    {
        public async Task<BodyMetric?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => await _context.BodyMetrics.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<IReadOnlyList<BodyMetric>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.BodyMetrics
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<BodyMetric> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _context.BodyMetrics.Where(x => x.UserId == userId)
                                            .OrderByDescending(x => x.Date);

            return await query.ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task AddAsync(BodyMetric bodyMetric, CancellationToken cancellationToken = default) => await _context.BodyMetrics.AddAsync(bodyMetric, cancellationToken);

        public void Update(BodyMetric bodyMetric) => _context.BodyMetrics.Update(bodyMetric);
    }
}