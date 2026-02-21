namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise
{
    public sealed record UpdateSplitExerciseCommand(Guid WorkoutProgramId,
                                                    Guid WorkoutProgramSplitId,
                                                    Guid WorkoutProgramExerciseId,
                                                    int Sets,
                                                    int MinimumReps,
                                                    int MaximumReps) : ICommand<Result<bool>>;
}