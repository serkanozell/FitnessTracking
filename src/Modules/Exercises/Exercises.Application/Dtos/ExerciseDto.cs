namespace Exercises.Application.Dtos
{
    public sealed class ExerciseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string PrimaryMuscleGroup { get; init; } = default!;
        public string? SecondaryMuscleGroup { get; init; }
        public string Description { get; init; } = default!;
    }
}