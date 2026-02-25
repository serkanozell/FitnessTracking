using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Application.Dtos
{
    public sealed class WorkoutSessionDto
    {
        public Guid Id { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public DateTime Date { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }

        public static WorkoutSessionDto FromEntity(WorkoutSession entity) =>
            new()
            {
                Id = entity.Id,
                WorkoutProgramId = entity.WorkoutProgramId,
                Date = entity.Date,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy
            };
    }
}