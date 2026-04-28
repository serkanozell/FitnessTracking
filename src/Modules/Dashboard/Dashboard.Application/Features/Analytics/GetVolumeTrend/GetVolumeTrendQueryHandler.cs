using BuildingBlocks.Application.Abstractions;
using Dashboard.Application.Dtos;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetVolumeTrend
{
    internal sealed class GetVolumeTrendQueryHandler(IWorkoutSessionModule _sessionModule,
                                                     ICurrentUser _currentUser)
        : IQueryHandler<GetVolumeTrendQuery, Result<IReadOnlyList<VolumeTrendPointDto>>>
    {
        public async Task<Result<IReadOnlyList<VolumeTrendPointDto>>> Handle(GetVolumeTrendQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);
            var dateTo = DateTime.Today.AddDays(1);
            var dateFrom = DateTime.Today.AddDays(-request.Days);

            var data = await _sessionModule.GetVolumeTrendAsync(userId, dateFrom, dateTo, request.Period, cancellationToken);

            var result = data.Select(p => new VolumeTrendPointDto
            {
                Date = p.Date,
                TotalVolume = p.TotalVolume,
                SessionCount = p.SessionCount,
                TotalSets = p.TotalSets,
                TotalReps = p.TotalReps
            }).ToList();

            return Result<IReadOnlyList<VolumeTrendPointDto>>.Success(result);
        }
    }
}
