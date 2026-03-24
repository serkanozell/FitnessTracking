namespace FitnessTracking.Mvc.Models;

public sealed class DashboardDto
{
    public ActiveProgramDto? ActiveProgram { get; init; }
    public WeeklyWorkoutsDto WeeklyWorkouts { get; init; } = new();
    public LatestBodyMetricDto? LatestBodyMetric { get; init; }
    public WorkoutStatsDto Stats { get; init; } = new();
}

public sealed class ActiveProgramDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DayCount { get; init; }
    public double CompletionPercentage { get; init; }
}

public sealed class WeeklyWorkoutsDto
{
    public int Completed { get; init; }
    public int StreakDays { get; init; }
}

public sealed class LatestBodyMetricDto
{
    public DateTime Date { get; init; }
    public decimal? Weight { get; init; }
    public decimal? BodyFatPercentage { get; init; }
    public decimal? MuscleMass { get; init; }
}

public sealed class WorkoutStatsDto
{
    public int TotalWorkouts { get; init; }
    public int TotalSets { get; init; }
    public int TotalReps { get; init; }
}

public sealed class WeightTrendDto
{
    public DateTime Date { get; init; }
    public decimal Weight { get; init; }
}
