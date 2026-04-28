namespace Dashboard.Application.Dtos;

public sealed class ExerciseProgressPointDto
{
    public DateTime Date { get; init; }
    public decimal MaxWeight { get; init; }
    public int MaxReps { get; init; }
    public decimal TotalVolume { get; init; }
    public decimal Estimated1Rm { get; init; }
}
