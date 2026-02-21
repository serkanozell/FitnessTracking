namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    public sealed record AddExerciseToSplitCommand(Guid WorkoutProgramId,
                                                   Guid WorkoutProgramSplitId,
                                                   Guid ExerciseId,
                                                   int Sets,
                                                   int MinimumReps,
                                                   int MaximumReps) : ICommand<Result<Guid>>;
}