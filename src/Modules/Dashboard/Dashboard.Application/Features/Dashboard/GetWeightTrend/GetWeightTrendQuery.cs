using Dashboard.Application.Dtos;

namespace Dashboard.Application.Features.Dashboard.GetWeightTrend;

public sealed record GetWeightTrendQuery(int Days = 90) : IQuery<Result<IReadOnlyList<WeightTrendDto>>>;