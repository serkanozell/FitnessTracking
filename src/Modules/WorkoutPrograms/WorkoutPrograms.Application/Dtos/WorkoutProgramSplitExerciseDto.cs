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
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }
    }
}