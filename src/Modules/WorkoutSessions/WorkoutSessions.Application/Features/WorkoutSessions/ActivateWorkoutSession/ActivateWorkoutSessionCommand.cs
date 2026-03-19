using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.ActivateWorkoutSession
{
    public sealed record ActivateWorkoutSessionCommand(Guid WorkoutSessionId) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{WorkoutSessionId}"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all"];
    }
}