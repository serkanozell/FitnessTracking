namespace Nutrition.Application.Errors
{
    public static class FoodErrors
    {
        public static Error NotFound(Guid id) =>
            new("Food.NotFound", $"Food with ID '{id}' was not found.");

        public static Error AlreadyActive(Guid id) =>
            new("Food.AlreadyActive", $"Food with ID '{id}' is already active.");

        public static Error AlreadyDeleted(Guid id) =>
            new("Food.AlreadyDeleted", $"Food with ID '{id}' is already deleted.");

        public static Error DuplicateName(string name) =>
            new("Food.DuplicateName", $"Food with name '{name}' already exists.");
    }
}
