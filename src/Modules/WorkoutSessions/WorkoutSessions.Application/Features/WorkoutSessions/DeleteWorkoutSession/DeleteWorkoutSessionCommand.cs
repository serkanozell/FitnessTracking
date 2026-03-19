using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession
{
    public sealed record DeleteWorkoutSessionCommand(Guid Id) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{Id}"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all", "workoutsessions:program:"];
    }
}