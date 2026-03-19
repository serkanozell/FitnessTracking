namespace FitnessTracking.Mvc.Models;

public sealed class WorkoutSessionDto
{
    public Guid Id { get; set; }
    public Guid WorkoutProgramId { get; set; }
    public DateTime Date { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public IReadOnlyList<WorkoutExerciseDto> Exercises { get; set; } = [];
}

public sealed class WorkoutSessionEditModel
{
    public Guid WorkoutProgramId { get; set; }
    public DateTime Date { get; set; } = DateTime.Today;
}

public sealed class WorkoutSessionDetailsDto
{
    public Guid Id { get; init; }
    public Guid WorkoutProgramId { get; init; }
    public DateTime Date { get; init; }
    public IReadOnlyList<WorkoutExerciseDto> Exercises { get; init; } = [];
}

public sealed class WorkoutExerciseDto
{
    public Guid Id { get; init; }
    public Guid ExerciseId { get; init; }
    public int SetNumber { get; init; }
    public decimal Weight { get; init; }
    public int Reps { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
}

public sealed class WorkoutExerciseEditModel
{
    public Guid ExerciseId { get; set; }
    public int SetNumber { get; set; }
    public decimal Weight { get; set; }
    public int Reps { get; set; }
}

public sealed class WorkoutExerciseAddResult
{
    public Guid SessionExerciseId { get; set; }
}
