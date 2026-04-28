using BuildingBlocks.Application.Abstractions;
using Dashboard.Application.Dtos;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetExerciseProgress
{
    internal sealed class GetExerciseProgressQueryHandler(IWorkoutSessionModule _sessionModule,
                                                          ICurrentUser _currentUser)
        : IQueryHandler<GetExerciseProgressQuery, Result<IReadOnlyList<ExerciseProgressPointDto>>>
    {
        public async Task<Result<IReadOnlyList<ExerciseProgressPointDto>>> Handle(GetExerciseProgressQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);
            var dateTo = DateTime.Today.AddDays(1);
            var dateFrom = DateTime.Today.AddDays(-request.Days);

            var data = await _sessionModule.GetExerciseProgressAsync(userId, request.ExerciseId, dateFrom, dateTo, cancellationToken);

            var result = data.Select(p => new ExerciseProgressPointDto
            {
                Date = p.Date,
                MaxWeight = p.MaxWeight,
                MaxReps = p.MaxReps,
                TotalVolume = p.TotalVolume,
                Estimated1Rm = p.Estimated1Rm
            }).ToList();

            return Result<IReadOnlyList<ExerciseProgressPointDto>>.Success(result);
        }
    }
}
