using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions
{
    public sealed record GetWorkoutSessionsQuery : IQuery<Result<IReadOnlyList<WorkoutSessionDto>>>, ICacheableQuery
    {
        public string CacheKey => "workoutsessions:all";
        public TimeSpan? Expiration => null;
    }
}