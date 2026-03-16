using BuildingBlocks.Application.Abstractions;
using BodyMetrics.Contracts;
using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Dashboard.GetWeightTrend
{
    internal sealed class GetWeightTrendQueryHandler(IBodyMetricModule _bodyMetricModule,
                                                     ICurrentUser _currentUser) : IQueryHandler<GetWeightTrendQuery, Result<IReadOnlyList<WeightTrendDto>>>
    {
        public async Task<Result<IReadOnlyList<WeightTrendDto>>> Handle(GetWeightTrendQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);
            var dateTo = DateTime.Today.AddDays(1);
            var dateFrom = DateTime.Today.AddDays(-request.Days);

            var trend = await _bodyMetricModule.GetWeightTrendAsync(userId, dateFrom, dateTo, cancellationToken);

            var result = trend.Select(t => new WeightTrendDto
            {
                Date = t.Date,
                Weight = t.Weight
            }).ToList();

            return Result<IReadOnlyList<WeightTrendDto>>.Success(result);
        }
    }
}