namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise
{
    public sealed class UpdateSplitExerciseCommand : ICommand<bool>
    {
        public Guid WorkoutProgramId { get; init; }
        public Guid WorkoutProgramSplitId { get; init; }
        public Guid WorkoutProgramExerciseId { get; init; }
        public int Sets { get; init; }
        public int TargetReps { get; init; }
    }
}