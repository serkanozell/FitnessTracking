using BuildingBlocks.Application.Pagination;
using BodyMetrics.Application.Dtos;

namespace BodyMetrics.Application.Features.BodyMetrics.GetBodyMetrics
{
    public sealed record GetBodyMetricsQuery(int PageNumber, int PageSize) : IQuery<Result<PagedResult<BodyMetricDto>>>;
}