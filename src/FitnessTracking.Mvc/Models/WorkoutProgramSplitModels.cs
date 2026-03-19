namespace FitnessTracking.Mvc.Models;

public sealed class WorkoutProgramSplitDto
{
    public Guid Id { get; init; }
    public Guid WorkoutProgramId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Order { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public IReadOnlyList<WorkoutProgramExerciseDto> Exercises { get; init; } = [];
}

public sealed class WorkoutProgramExerciseDto
{
    public Guid WorkoutProgramExerciseId { get; init; }
    public Guid ExerciseId { get; init; }
    public string ExerciseName { get; init; } = string.Empty;
    public int Sets { get; init; }
    public int MinimumReps { get; init; }
    public int MaximumReps { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
}

public sealed class AddSplitRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public sealed class UpdateSplitRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public sealed class AddProgramExerciseRequest
{
    public Guid ExerciseId { get; set; }
    public int Sets { get; set; }
    public int MinimumReps { get; set; }
    public int MaximumReps { get; set; }
}

public sealed class UpdateProgramExerciseRequest
{
    public int Sets { get; set; }
    public int MinimumReps { get; set; }
    public int MaximumReps { get; set; }
}
