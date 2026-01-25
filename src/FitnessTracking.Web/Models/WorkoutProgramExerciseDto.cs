namespace FitnessTracking.Web.Models
{
    public sealed class WorkoutProgramExerciseDto
    {
        public Guid WorkoutProgramExerciseId { get; init; }
        public Guid ExerciseId { get; init; }
        public string ExerciseName { get; init; } = string.Empty;
        public int Sets { get; init; }
        public int TargetReps { get; init; }
    }
}