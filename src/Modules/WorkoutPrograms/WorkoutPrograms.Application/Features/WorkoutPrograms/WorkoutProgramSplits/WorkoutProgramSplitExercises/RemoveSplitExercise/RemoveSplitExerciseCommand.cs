namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    public sealed record RemoveSplitExerciseCommand(Guid WorkoutProgramId,
                                                    Guid WorkoutProgramSplitId,
                                                    Guid WorkoutProgramExerciseId) : ICommand<Result<bool>>;
}