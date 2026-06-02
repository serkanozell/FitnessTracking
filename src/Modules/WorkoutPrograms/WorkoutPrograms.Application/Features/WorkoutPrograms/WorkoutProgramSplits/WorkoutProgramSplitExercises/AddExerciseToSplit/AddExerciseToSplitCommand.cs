namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record AddExerciseToSplitCommand(Guid WorkoutProgramId,
                                                   Guid WorkoutProgramSplitId,
                                                   Guid ExerciseId,
                                                   int Sets,
                                                   int MinimumReps,
                                                   int MaximumReps,
                                                   string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}