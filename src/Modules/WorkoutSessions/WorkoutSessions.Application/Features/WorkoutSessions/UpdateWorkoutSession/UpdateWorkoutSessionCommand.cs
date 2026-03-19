using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.UpdateWorkoutSession
{
    public sealed record UpdateWorkoutSessionCommand(Guid Id,
                                                     DateTime Date) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{Id}"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all", "workoutsessions:program:"];
    }
}