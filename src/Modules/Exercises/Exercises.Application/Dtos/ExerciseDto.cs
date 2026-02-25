namespace Exercises.Application.Dtos
{
    public sealed class ExerciseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string PrimaryMuscleGroup { get; init; } = default!;
        public string? SecondaryMuscleGroup { get; init; }
        public string Description { get; init; } = default!;
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }
    }
}