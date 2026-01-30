namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    public sealed class RemoveSplitExerciseCommand : ICommand<bool>
    {
        public Guid WorkoutProgramId { get; init; }
        public Guid WorkoutProgramSplitId { get; init; }
        public Guid WorkoutProgramExerciseId { get; init; }
    }
}