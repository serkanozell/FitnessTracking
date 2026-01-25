namespace FitnessTracking.Client.Services.WorkoutPrograms.Models
{
    public sealed class WorkoutProgramDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }

    public sealed class CreateWorkoutProgramRequest
    {
        public string Name { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }

    public sealed class UpdateWorkoutProgramRequest
    {
        public string Name { get; init; } = string.Empty;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }

    public sealed class AddProgramExerciseRequest
    {
        public Guid ExerciseId { get; init; }
        public int Sets { get; init; }
        public int TargetReps { get; init; }
    }

    public sealed class UpdateProgramExerciseRequest
    {
        public int Sets { get; init; }
        public int TargetReps { get; init; }
    }
}