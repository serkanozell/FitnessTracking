using WorkoutPrograms.Domain.Entity;

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

        public static WorkoutProgramSplitExerciseDto FromEntity(WorkoutSplitExercise entity, string exerciseName) =>
            new()
            {
                WorkoutProgramExerciseId = entity.Id,
                ExerciseId = entity.ExerciseId,
                ExerciseName = exerciseName,
                Sets = entity.Sets,
                MinimumReps = entity.RepRange.Minimum,
                MaximumReps = entity.RepRange.Maximum,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy
            };
    }
}