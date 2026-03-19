using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    public sealed record DeleteWorkoutProgramSplitCommand(Guid WorkoutProgramId, Guid SplitId) : ICommand<Result<bool>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits"];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}