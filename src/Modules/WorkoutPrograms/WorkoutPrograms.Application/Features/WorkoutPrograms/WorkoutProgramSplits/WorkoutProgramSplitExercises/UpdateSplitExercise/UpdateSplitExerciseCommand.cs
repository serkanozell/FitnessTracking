namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record UpdateSplitExerciseCommand(Guid WorkoutProgramId,
                                                    Guid WorkoutProgramSplitId,
                                                    Guid WorkoutProgramExerciseId,
                                                    int Sets,
                                                    int MinimumReps,
                                                    int MaximumReps) : ICommand<Result<bool>>;
}