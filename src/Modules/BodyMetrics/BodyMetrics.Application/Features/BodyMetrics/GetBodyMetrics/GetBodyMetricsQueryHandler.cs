using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BodyMetrics.Application.Dtos;

namespace BodyMetrics.Application.Features.BodyMetrics.GetBodyMetrics
{
    internal sealed class GetBodyMetricsQueryHandler(
        IBodyMetricRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetBodyMetricsQuery, Result<PagedResult<BodyMetricDto>>>
    {
        public async Task<Result<PagedResult<BodyMetricDto>>> Handle(GetBodyMetricsQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (dtos, totalCount) = await _repository.GetPagedByUserAsync(userId, request.PageNumber, request.PageSize, BodyMetricDto.Projection, cancellationToken);

            return PagedResult<BodyMetricDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}