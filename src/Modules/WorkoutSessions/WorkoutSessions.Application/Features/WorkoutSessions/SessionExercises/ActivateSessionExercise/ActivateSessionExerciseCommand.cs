using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    public sealed record ActivateSessionExerciseCommand(Guid WorkoutSessionId,
                                                        Guid SessionExerciseId) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{WorkoutSessionId}", $"workoutsessions:{WorkoutSessionId}:exercises"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all"];
    }
}