namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    // User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
    // nothing to invalidate here.
    public sealed record RemoveSplitExerciseCommand(Guid WorkoutProgramId,
                                                    Guid WorkoutProgramSplitId,
                                                    Guid WorkoutProgramExerciseId) : ICommand<Result<bool>>;
}