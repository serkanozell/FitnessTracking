namespace Dashboard.Application.Dtos;

public sealed class MuscleGroupVolumeDto
{
    public string MuscleGroup { get; init; } = string.Empty;
    public decimal TotalVolume { get; init; }
    public int SetCount { get; init; }
    public int TotalReps { get; init; }
}
