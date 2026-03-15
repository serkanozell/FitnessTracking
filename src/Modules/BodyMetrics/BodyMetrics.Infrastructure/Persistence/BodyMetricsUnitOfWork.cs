using BuildingBlocks.Infrastructure.Persistence;
using BodyMetrics.Domain.Repositories;

namespace BodyMetrics.Infrastructure.Persistence
{
    public sealed class BodyMetricsUnitOfWork(BodyMetricsDbContext context) : UnitOfWork<BodyMetricsDbContext>(context), IBodyMetricsUnitOfWork;
}