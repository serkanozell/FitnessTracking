using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Application.Dtos
{
    public sealed class SessionExerciseDto
    {
        public Guid Id { get; init; }
        public Guid ExerciseId { get; init; }
        public int SetNumber { get; init; }
        public decimal Weight { get; init; }
        public int Reps { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }

        public static SessionExerciseDto FromEntity(SessionExercise entity) =>
            new()
            {
                Id = entity.Id,
                ExerciseId = entity.ExerciseId,
                SetNumber = entity.SetNumber,
                Weight = entity.Weight,
                Reps = entity.Reps,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy,
            };
    }
}