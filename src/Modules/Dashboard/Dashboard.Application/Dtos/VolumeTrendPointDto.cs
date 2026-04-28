namespace Dashboard.Application.Dtos;

public sealed class VolumeTrendPointDto
{
    public DateTime Date { get; init; }
    public decimal TotalVolume { get; init; }
    public int SessionCount { get; init; }
    public int TotalSets { get; init; }
    public int TotalReps { get; init; }
}
