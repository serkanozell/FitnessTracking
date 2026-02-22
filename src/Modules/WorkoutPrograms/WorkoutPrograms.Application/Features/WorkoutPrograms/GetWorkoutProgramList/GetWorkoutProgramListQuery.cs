using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    public sealed record GetWorkoutProgramListQuery : IQuery<Result<IReadOnlyList<WorkoutProgramDto>>>, ICacheableQuery
    {
        public string CacheKey => "workoutprograms:all";
        public TimeSpan? Expiration => null;
    }
}