namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

public sealed record ActivateSplitExerciseCommand(Guid WorkoutProgramId,
                                                  Guid SplitId,
                                                  Guid WorkoutSplitExerciseId) : ICommand<Result<Guid>>;