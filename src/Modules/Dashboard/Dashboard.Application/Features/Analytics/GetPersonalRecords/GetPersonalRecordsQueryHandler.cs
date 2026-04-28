using BuildingBlocks.Application.Abstractions;
using Dashboard.Application.Dtos;
using Exercises.Contracts;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetPersonalRecords
{
    internal sealed class GetPersonalRecordsQueryHandler(IWorkoutSessionModule _sessionModule,
                                                         IExerciseModule _exerciseModule,
                                                         ICurrentUser _currentUser)
        : IQueryHandler<GetPersonalRecordsQuery, Result<IReadOnlyList<PersonalRecordDto>>>
    {
        public async Task<Result<IReadOnlyList<PersonalRecordDto>>> Handle(GetPersonalRecordsQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var prs = await _sessionModule.GetPersonalRecordsAsync(userId, request.Top, cancellationToken);

            if (prs.Count == 0)
            {
                return Result<IReadOnlyList<PersonalRecordDto>>.Success([]);
            }

            var exercises = await _exerciseModule.GetExercisesAsync(cancellationToken);
            var exerciseLookup = exercises.ToDictionary(e => e.Id);

            var result = prs.Select(p =>
            {
                exerciseLookup.TryGetValue(p.ExerciseId, out var info);
                return new PersonalRecordDto
                {
                    ExerciseId = p.ExerciseId,
                    ExerciseName = info?.Name ?? "Unknown",
                    PrimaryMuscleGroup = info?.PrimaryMuscleGroup,
                    MaxWeight = p.MaxWeight,
                    RepsAtMaxWeight = p.RepsAtMaxWeight,
                    Estimated1Rm = p.Estimated1Rm,
                    AchievedOn = p.AchievedOn
                };
            }).ToList();

            return Result<IReadOnlyList<PersonalRecordDto>>.Success(result);
        }
    }
}
