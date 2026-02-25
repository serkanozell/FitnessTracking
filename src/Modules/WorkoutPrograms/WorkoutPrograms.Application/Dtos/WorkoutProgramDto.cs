using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Application.Dtos
{
    public sealed class WorkoutProgramDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }

        public static WorkoutProgramDto FromEntity(WorkoutProgram entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy
            };
    }
}