using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById
{
    public sealed record GetWorkoutProgramByIdQuery(Guid Id) : IQuery<Result<WorkoutProgramDto>>, ICacheableQuery
    {
        public string CacheKey => $"workoutprograms:{Id}";
        public TimeSpan? Expiration => null;
    }
}