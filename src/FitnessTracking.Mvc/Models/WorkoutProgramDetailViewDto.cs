namespace FitnessTracking.Mvc.Models;

public sealed class WorkoutProgramDetailViewDto
{
    public WorkoutProgramDto Program { get; set; } = new();
    public IReadOnlyList<ExerciseLookupDto> AllExercises { get; set; } = [];
}

public sealed class ExerciseLookupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
