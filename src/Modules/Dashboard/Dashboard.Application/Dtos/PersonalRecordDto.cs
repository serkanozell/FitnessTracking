namespace Dashboard.Application.Dtos;

public sealed class PersonalRecordDto
{
    public Guid ExerciseId { get; init; }
    public string ExerciseName { get; init; } = string.Empty;
    public string? PrimaryMuscleGroup { get; init; }
    public decimal MaxWeight { get; init; }
    public int RepsAtMaxWeight { get; init; }
    public decimal Estimated1Rm { get; init; }
    public DateTime AchievedOn { get; init; }
}
