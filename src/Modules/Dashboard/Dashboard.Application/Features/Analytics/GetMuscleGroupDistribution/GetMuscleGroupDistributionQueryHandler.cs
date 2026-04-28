using BuildingBlocks.Application.Abstractions;
using Dashboard.Application.Dtos;
using Exercises.Contracts;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetMuscleGroupDistribution
{
    internal sealed class GetMuscleGroupDistributionQueryHandler(IWorkoutSessionModule _sessionModule,
                                                                 IExerciseModule _exerciseModule,
                                                                 ICurrentUser _currentUser)
        : IQueryHandler<GetMuscleGroupDistributionQuery, Result<IReadOnlyList<MuscleGroupVolumeDto>>>
    {
        public async Task<Result<IReadOnlyList<MuscleGroupVolumeDto>>> Handle(GetMuscleGroupDistributionQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);
            var dateTo = DateTime.Today.AddDays(1);
            var dateFrom = DateTime.Today.AddDays(-request.Days);

            var breakdown = await _sessionModule.GetExerciseVolumeBreakdownAsync(userId, dateFrom, dateTo, cancellationToken);

            if (breakdown.Count == 0)
            {
                return Result<IReadOnlyList<MuscleGroupVolumeDto>>.Success([]);
            }

            var exercises = await _exerciseModule.GetExercisesAsync(cancellationToken);
            var muscleByExercise = exercises.ToDictionary(e => e.Id, e => e.PrimaryMuscleGroup);

            var grouped = breakdown
                .GroupBy(b => muscleByExercise.TryGetValue(b.ExerciseId, out var mg) ? mg : "Unknown")
                .Select(g => new MuscleGroupVolumeDto
                {
                    MuscleGroup = g.Key,
                    TotalVolume = g.Sum(x => x.TotalVolume),
                    SetCount = g.Sum(x => x.SetCount),
                    TotalReps = g.Sum(x => x.TotalReps)
                })
                .OrderByDescending(x => x.TotalVolume)
                .ToList();

            return Result<IReadOnlyList<MuscleGroupVolumeDto>>.Success(grouped);
        }
    }
}
