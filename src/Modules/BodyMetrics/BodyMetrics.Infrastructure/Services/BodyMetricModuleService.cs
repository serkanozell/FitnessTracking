using BodyMetrics.Contracts;
using BodyMetrics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BodyMetrics.Infrastructure.Services
{
    internal sealed class BodyMetricModuleService(BodyMetricsDbContext _context) : IBodyMetricModule
    {
        public async Task<LatestBodyMetricInfo?> GetLatestByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var metric = await _context.BodyMetrics.Where(x => x.UserId == userId && !x.IsDeleted)
                                                   .OrderByDescending(x => x.Date)
                                                   .FirstOrDefaultAsync(cancellationToken);

            if (metric is null) return null;

            return new LatestBodyMetricInfo(metric.Date, metric.Weight?.Value, metric.BodyFatPercentage?.Value, metric.MuscleMass);
        }

        public async Task<IReadOnlyList<WeightTrendPoint>> GetWeightTrendAsync(Guid userId, DateTime dateFrom, DateTime dateTo, CancellationToken cancellationToken = default)
        {
            return await _context.BodyMetrics.Where(x => x.UserId == userId && x.IsActive && !x.IsDeleted && x.Weight != null && x.Date >= dateFrom && x.Date <= dateTo)
                                             .OrderBy(x => x.Date)
                                             .Select(x => new WeightTrendPoint(x.Date, x.Weight!.Value))
                                             .AsNoTracking()
                                             .ToListAsync(cancellationToken);
        }
    }
}