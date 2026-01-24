public sealed class ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string MuscleGroup { get; init; } = default!;
    public string Description { get; init; } = default!;
}