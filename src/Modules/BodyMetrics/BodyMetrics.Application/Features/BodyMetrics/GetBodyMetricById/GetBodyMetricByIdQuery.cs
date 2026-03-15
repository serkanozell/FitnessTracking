using BodyMetrics.Application.Dtos;

namespace BodyMetrics.Application.Features.BodyMetrics.GetBodyMetricById
{
    public sealed record GetBodyMetricByIdQuery(Guid Id) : IQuery<Result<BodyMetricDto>>;
}