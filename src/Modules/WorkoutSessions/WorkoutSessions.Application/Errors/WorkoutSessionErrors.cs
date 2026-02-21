namespace WorkoutSessions.Application.Errors;

public static class WorkoutSessionErrors
{
    public static Error NotFound(Guid id) =>
        new("WorkoutSession.NotFound", $"Workout session with ID '{id}' was not found.");

    public static Error AlreadyActive(Guid id) =>
        new("WorkoutSession.AlreadyActive", $"Workout session with ID '{id}' is already active.");

    public static Error NotActive(Guid id) =>
        new("WorkoutSession.NotActive", $"Workout session with ID '{id}' is not active.");

    public static Error ProgramNotFound(Guid programId) =>
        new("WorkoutSession.ProgramNotFound", $"Workout program with ID '{programId}' was not found.");

    public static Error ExerciseNotInProgram(Guid exerciseId, Guid programId) =>
        new("WorkoutSession.ExerciseNotInProgram", $"Exercise '{exerciseId}' is not part of workout program '{programId}'.");

    public static Error SetLimitExceeded(Guid exerciseId, int maxSets, int currentSets) =>
        new("WorkoutSession.SetLimitExceeded", $"Exercise '{exerciseId}' is limited to {maxSets} sets. Current: {currentSets}.");

    public static Error SetNumberExceedsLimit(Guid exerciseId, int maxSets) =>
        new("WorkoutSession.SetNumberExceedsLimit", $"Exercise '{exerciseId}' is limited to {maxSets} sets.");

    public static Error DuplicateSet(Guid exerciseId, int setNumber) =>
        new("WorkoutSession.DuplicateSet", $"Set {setNumber} for exercise '{exerciseId}' already exists.");

    public static Error SessionExerciseNotFound(Guid sessionId, Guid sessionExerciseId) =>
        new("WorkoutSession.SessionExerciseNotFound", $"Session exercise '{sessionExerciseId}' not found in session '{sessionId}'.");
}