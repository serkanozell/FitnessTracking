using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    public sealed record RemoveExerciseFromSessionCommand(Guid WorkoutSessionId,
                                                          Guid SessionExerciseId) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{WorkoutSessionId}", $"workoutsessions:{WorkoutSessionId}:exercises"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all"];
    }
}