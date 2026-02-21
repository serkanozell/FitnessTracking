using BuildingBlocks.Application.Results;

namespace WorkoutPrograms.Application.Errors
{
    public static class WorkoutProgramErrors
    {
        public static Error NotFound(Guid id) =>
            new("WorkoutProgram.NotFound", $"Workout program with ID '{id}' was not found.");

        public static Error AlreadyActive(Guid id) =>
            new("WorkoutProgram.AlreadyActive", $"Workout program with ID '{id}' is already active.");

        public static Error AlreadyDeleted(Guid id) =>
            new("WorkoutProgram.AlreadyDeleted", $"Workout program with ID '{id}' is already deleted.");

        public static Error NotActive(Guid id) =>
            new("WorkoutProgram.NotActive", $"Workout program with ID '{id}' is not active.");

        public static Error DuplicateName(string name) =>
            new("WorkoutProgram.DuplicateName", $"Workout program with name '{name}' already exists.");

        public static Error InvalidDateRange() =>
            new("WorkoutProgram.InvalidDateRange", "End date must be greater than start date.");

        // Split Errors
        public static Error SplitNotFound(Guid programId, Guid splitId) =>
            new("WorkoutProgram.SplitNotFound", $"Split with ID '{splitId}' was not found in program '{programId}'.");

        public static Error SplitDuplicateName(string name) =>
            new("WorkoutProgram.SplitDuplicateName", $"Split with name '{name}' already exists in this program.");

        // Exercise Errors
        public static Error ExerciseNotFoundInSplit(Guid splitId, Guid exerciseId) =>
            new("WorkoutProgram.ExerciseNotFoundInSplit", $"Exercise with ID '{exerciseId}' was not found in split '{splitId}'.");
    }
}