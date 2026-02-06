namespace WorkoutPrograms.Application.Dtos
{
    public sealed class WorkoutProgramSplitExerciseDto
    {
        public Guid WorkoutProgramExerciseId { get; init; }
        public Guid ExerciseId { get; init; }
        public string ExerciseName { get; init; } = default!;
        public int Sets { get; init; }
        public int MinimumReps { get; init; }
        public int MaximumReps { get; init; }
    }
}