using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    public sealed record AddExerciseToSessionCommand(Guid WorkoutSessionId,
                                                     Guid ExerciseId,
                                                     int SetNumber,
                                                     decimal Weight,
                                                     int Reps) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutsessions:{WorkoutSessionId}", $"workoutsessions:{WorkoutSessionId}:exercises"];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all"];
    }
}