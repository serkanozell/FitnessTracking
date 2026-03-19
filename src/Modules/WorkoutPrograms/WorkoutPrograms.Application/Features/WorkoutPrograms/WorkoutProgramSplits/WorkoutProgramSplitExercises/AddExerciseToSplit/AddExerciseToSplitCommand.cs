using BuildingBlocks.Application.Abstractions.Caching;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    public sealed record AddExerciseToSplitCommand(Guid WorkoutProgramId,
                                                   Guid WorkoutProgramSplitId,
                                                   Guid ExerciseId,
                                                   int Sets,
                                                   int MinimumReps,
                                                   int MaximumReps) : ICommand<Result<Guid>>, ICacheInvalidatingCommand
    {
        public string[] CacheKeysToInvalidate => [$"workoutprograms:{WorkoutProgramId}", $"workoutprograms:{WorkoutProgramId}:splits:{WorkoutProgramSplitId}:exercises"];
        public string[] CachePrefixesToInvalidate => ["workoutprograms:all"];
    }
}