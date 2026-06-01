namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

// User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
// nothing to invalidate here.
public sealed record ActivateSplitExerciseCommand(Guid WorkoutProgramId,
                                                  Guid SplitId,
                                                  Guid WorkoutSplitExerciseId) : ICommand<Result<Guid>>;