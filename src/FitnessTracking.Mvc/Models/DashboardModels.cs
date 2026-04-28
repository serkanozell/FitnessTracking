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

public enum AnalyticsGroupingPeriod
{
    Day = 0,
    Week = 1,
    Month = 2
}

public sealed class VolumeTrendPointDto
{
    public DateTime Date { get; init; }
    public decimal TotalVolume { get; init; }
    public int SessionCount { get; init; }
    public int TotalSets { get; init; }
    public int TotalReps { get; init; }
}

public sealed class ExerciseProgressPointDto
{
    public DateTime Date { get; init; }
    public decimal MaxWeight { get; init; }
    public int MaxReps { get; init; }
    public decimal TotalVolume { get; init; }
    public decimal Estimated1Rm { get; init; }
}

public sealed class MuscleGroupVolumeDto
{
    public string MuscleGroup { get; init; } = string.Empty;
    public decimal TotalVolume { get; init; }
    public int SetCount { get; init; }
    public int TotalReps { get; init; }
}

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

public sealed class AnalyticsViewModel
{
    public int Days { get; init; } = 30;
    public AnalyticsGroupingPeriod Period { get; init; } = AnalyticsGroupingPeriod.Day;
    public Guid? ExerciseId { get; init; }

    public IReadOnlyList<ExerciseDto> Exercises { get; init; } = [];
    public IReadOnlyList<VolumeTrendPointDto> VolumeTrend { get; init; } = [];
    public IReadOnlyList<MuscleGroupVolumeDto> MuscleGroupDistribution { get; init; } = [];
    public IReadOnlyList<ExerciseProgressPointDto> ExerciseProgress { get; init; } = [];
    public IReadOnlyList<PersonalRecordDto> PersonalRecords { get; init; } = [];
}
