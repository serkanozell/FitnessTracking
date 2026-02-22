using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    public sealed record GetWorkoutProgramSplitsQuery(Guid WorkoutProgramId) : IQuery<Result<IReadOnlyList<WorkoutProgramSplitDto>>>, ICacheableQuery
    {
        public string CacheKey => $"workoutprograms:{WorkoutProgramId}:splits";
        public TimeSpan? Expiration => null;
    }
}