using BuildingBlocks.Application.Abstractions;
using BodyMetrics.Application.Dtos;

namespace BodyMetrics.Application.Features.BodyMetrics.GetBodyMetricById
{
    internal sealed class GetBodyMetricByIdQueryHandler(
        IBodyMetricRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetBodyMetricByIdQuery, Result<BodyMetricDto>>
    {
        public async Task<Result<BodyMetricDto>> Handle(GetBodyMetricByIdQuery request, CancellationToken cancellationToken)
        {
            var metric = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (metric is null)
                return BodyMetricErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, metric.UserId);
            if (ownershipError is not null)
                return ownershipError;

            return BodyMetricDto.FromEntity(metric);
        }
    }
}