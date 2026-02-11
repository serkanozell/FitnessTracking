namespace Exercises.Application.Errors
{
    public static class ExerciseErrors
    {
        public static Error NotFound(Guid id) => new("Exercise.NotFound", $"Exercise with ID '{id}' was not found.");

        public static Error AlreadyActive(Guid id) => new("Exercise.AlreadyActive", $"Exercise with ID '{id}' is already active.");

        public static Error AlreadyDeleted(Guid id) => new("Exercise.AlreadyDeleted", $"Exercise with ID '{id}' is already deleted.");

        public static Error DuplicateName(string name) => new("Exercise.DuplicateName", $"Exercise with name '{name}' already exists.");
    }
}