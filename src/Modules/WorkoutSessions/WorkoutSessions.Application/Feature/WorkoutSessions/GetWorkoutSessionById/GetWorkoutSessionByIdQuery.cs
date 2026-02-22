using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById
{
    public sealed record GetWorkoutSessionByIdQuery(Guid Id) : IQuery<Result<WorkoutSessionDetailDto>>, ICacheableQuery
    {
        public string CacheKey => $"workoutsessions:{Id}";
        public TimeSpan? Expiration => null;
    }
}