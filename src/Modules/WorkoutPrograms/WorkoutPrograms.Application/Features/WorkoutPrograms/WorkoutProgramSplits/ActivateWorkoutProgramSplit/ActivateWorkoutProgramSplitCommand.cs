namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

public sealed record ActivateWorkoutProgramSplitCommand(Guid WorkoutProgramId,
                                                        Guid SplitId) : ICommand<Result<Guid>>;