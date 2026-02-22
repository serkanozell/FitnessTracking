using BuildingBlocks.Application.Abstractions.Caching;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram
{
    public sealed record GetWorkoutSessionsByProgramQuery(Guid WorkoutProgramId) : IQuery<Result<IReadOnlyList<WorkoutSessionDto>>>, ICacheableQuery
    {
        public string CacheKey => $"workoutsessions:program:{WorkoutProgramId}";
        public TimeSpan? Expiration => null;
    }
}