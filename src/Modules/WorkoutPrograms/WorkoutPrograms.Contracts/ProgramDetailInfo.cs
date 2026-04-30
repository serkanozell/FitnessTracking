namespace WorkoutPrograms.Contracts;

public record ProgramDetailInfo(
    Guid Id,
    string Name,
    IReadOnlyList<ProgramSplitSummaryInfo> Splits);

public record ProgramSplitSummaryInfo(
    Guid Id,
    string Name,
    int Order,
    bool IsDeleted,
    IReadOnlyList<ProgramSplitExerciseInfo> Exercises);

public record ProgramSplitExerciseInfo(
    Guid ExerciseId,
    bool IsDeleted);
