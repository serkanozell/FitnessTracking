namespace FitnessTracking.Mvc.Models;

public sealed class ExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PrimaryMuscleGroup { get; init; } = string.Empty;
    public string? SecondaryMuscleGroup { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? CreatedDate { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? UpdatedDate { get; init; }
    public string? UpdatedBy { get; init; }
}
