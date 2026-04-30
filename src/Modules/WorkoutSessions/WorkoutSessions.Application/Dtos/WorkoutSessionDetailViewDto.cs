namespace WorkoutSessions.Application.Dtos;

public sealed class WorkoutSessionDetailViewDto
{
    public WorkoutSessionDetailDto Session { get; init; } = default!;
    public string ProgramName { get; init; } = string.Empty;
    public IReadOnlyList<SessionSplitInfoDto> ProgramSplits { get; init; } = [];
    public IReadOnlyList<SessionSplitExerciseInfoDto> SessionSplitExercises { get; init; } = [];
    public IReadOnlyDictionary<Guid, string> ExerciseNames { get; init; } = new Dictionary<Guid, string>();
}

public sealed record SessionSplitInfoDto(Guid Id, string Name, int Order);

public sealed record SessionSplitExerciseInfoDto(Guid ExerciseId, string Name);
