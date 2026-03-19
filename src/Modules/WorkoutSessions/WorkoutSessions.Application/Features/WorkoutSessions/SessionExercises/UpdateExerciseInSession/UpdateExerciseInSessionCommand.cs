using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.UpdateExerciseInSession
{
    public sealed record UpdateExerciseInSessionCommand(Guid WorkoutSessionId,
                                                        Guid SessionExerciseId,
                                                        int SetNumber,
                                                        decimal Weight,
                                                        int Reps) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{WorkoutSessionId}", $"workoutsessions:{WorkoutSessionId}:exercises"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all"];
    }
}