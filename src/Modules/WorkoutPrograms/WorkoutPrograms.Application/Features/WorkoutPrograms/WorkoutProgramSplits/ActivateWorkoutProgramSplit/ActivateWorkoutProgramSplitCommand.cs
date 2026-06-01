namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

// User-scoped queries are not cached (see docs/ARCHITECTURE.md), so there is
// nothing to invalidate here.
public sealed record ActivateWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                        Guid SplitId) : ICommand<Result<Guid>>;