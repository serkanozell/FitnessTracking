using Exercises.Domain.Entity;

namespace Exercises.Application.Dtos
{
    public sealed class ExerciseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string PrimaryMuscleGroup { get; init; } = default!;
        public string? SecondaryMuscleGroup { get; init; }
        public string Description { get; init; } = default!;
        public string? ImageUrl { get; init; }
        public string? VideoUrl { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }

        public static ExerciseDto FromEntity(Exercise entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                PrimaryMuscleGroup = entity.PrimaryMuscleGroup.ToString(),
                SecondaryMuscleGroup = entity.SecondaryMuscleGroup?.ToString(),
                Description = entity.Description,
                ImageUrl = entity.ImageUrl,
                VideoUrl = entity.VideoUrl,
                IsActive = entity.IsActive,
                IsDeleted = entity.IsDeleted,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy,
                UpdatedDate = entity.UpdatedDate,
                UpdatedBy = entity.UpdatedBy
            };
    }
}