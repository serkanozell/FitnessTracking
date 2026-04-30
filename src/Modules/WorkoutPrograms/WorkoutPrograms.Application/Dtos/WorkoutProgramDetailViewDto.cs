namespace WorkoutPrograms.Application.Dtos;

public sealed class WorkoutProgramDetailViewDto
{
    public WorkoutProgramDto Program { get; init; } = default!;
    public IReadOnlyList<ExerciseLookupDto> AllExercises { get; init; } = [];
}

public sealed record ExerciseLookupDto(Guid Id, string Name);
