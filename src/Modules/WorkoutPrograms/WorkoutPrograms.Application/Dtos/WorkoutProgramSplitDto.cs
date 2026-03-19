using Exercises.Contracts;
using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Application.Dtos
{
    public sealed class WorkoutProgramSplitDto
    {
        public Guid Id { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public string Name { get; init; } = default!;
        public int Order { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }
        public IReadOnlyList<WorkoutProgramSplitExerciseDto> Exercises { get; init; } = [];

        public static WorkoutProgramSplitDto FromEntity(WorkoutProgramSplit entity) =>
            new()
            {
                Id = entity.Id,
                WorkoutProgramId = entity.WorkoutProgramId,
                Name = entity.Name,
                Order = entity.Order,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy
            };

        public static WorkoutProgramSplitDto FromEntity(WorkoutProgramSplit entity, IReadOnlyCollection<ExerciseInfo> allExercises) =>
            new()
            {
                Id = entity.Id,
                WorkoutProgramId = entity.WorkoutProgramId,
                Name = entity.Name,
                Order = entity.Order,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy,
                Exercises = entity.Exercises
                    .Select(e => WorkoutProgramSplitExerciseDto.FromEntity(e,
                        allExercises.FirstOrDefault(ex => ex.Id == e.ExerciseId)?.Name ?? string.Empty))
                    .ToList()
            };
    }
}