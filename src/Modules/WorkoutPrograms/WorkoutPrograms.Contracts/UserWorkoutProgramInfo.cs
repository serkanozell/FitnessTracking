namespace WorkoutPrograms.Contracts;

public record UserWorkoutProgramInfo(
    Guid Id,
    Guid UserId,
    string Name,
    string? Description,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive,
    bool IsDeleted,
    IReadOnlyList<UserWorkoutProgramSplitInfo> Splits);

public record UserWorkoutProgramSplitInfo(
    Guid Id,
    string Name,
    int Order,
    bool IsDeleted,
    IReadOnlyList<UserWorkoutProgramSplitExerciseInfo> Exercises);

public record UserWorkoutProgramSplitExerciseInfo(
    Guid ExerciseId,
    bool IsActive,
    bool IsDeleted);
