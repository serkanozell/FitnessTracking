using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    public sealed record GetSplitExercisesQuery(Guid WorkoutProgramId,
                                                Guid WorkoutSplitId) : IQuery<Result<IReadOnlyList<WorkoutProgramSplitExerciseDto>>>, ICacheableQuery
    {
        public string CacheKey => $"workoutprograms:{WorkoutProgramId}:splits:{WorkoutSplitId}:exercises";
        public TimeSpan? Expiration => null;
    }
}