using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession
{
    public sealed record CreateWorkoutSessionCommand(Guid WorkoutProgramId,
                                                     DateTime Date) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [];
        public string[] CachePrefixesToInvalidate => ["workoutsessions:all", $"workoutsessions:program:{WorkoutProgramId}"];
    }
}