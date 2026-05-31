namespace Dashboard.Application.Dtos;

public sealed class AnalyticsPageDto
{
    public int Days { get; init; }
    public GroupingPeriodDto Period { get; init; }
    public Guid? ExerciseId { get; init; }
    public Guid? ProgramId { get; init; }
    public Guid? SplitId { get; init; }

    public IReadOnlyList<AnalyticsExerciseDto> Exercises { get; init; } = [];
    public IReadOnlyList<AnalyticsProgramDto> Programs { get; init; } = [];
    public IReadOnlyList<VolumeTrendPointDto> VolumeTrend { get; init; } = [];
    public IReadOnlyList<MuscleGroupVolumeDto> MuscleGroupDistribution { get; init; } = [];
    public IReadOnlyList<ExerciseProgressPointDto> ExerciseProgress { get; init; } = [];
    public IReadOnlyList<PersonalRecordDto> PersonalRecords { get; init; } = [];
}

public enum GroupingPeriodDto
{
    Day = 0,
    Week = 1,
    Month = 2
}

public sealed class AnalyticsExerciseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string PrimaryMuscleGroup { get; init; } = string.Empty;
}

public sealed class AnalyticsProgramDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public IReadOnlyList<AnalyticsProgramSplitDto> Splits { get; init; } = [];
}

public sealed class AnalyticsProgramSplitDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public bool IsDeleted { get; init; }
}
