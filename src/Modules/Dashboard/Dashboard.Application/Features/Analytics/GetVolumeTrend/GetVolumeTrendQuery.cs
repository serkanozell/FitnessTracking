using Dashboard.Application.Dtos;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetVolumeTrend;

public sealed record GetVolumeTrendQuery(int Days = 30, GroupingPeriod Period = GroupingPeriod.Day)
    : IQuery<Result<IReadOnlyList<VolumeTrendPointDto>>>;
