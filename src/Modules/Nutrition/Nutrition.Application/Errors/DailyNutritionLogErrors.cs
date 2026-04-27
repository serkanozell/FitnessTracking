namespace Nutrition.Application.Errors
{
    public static class DailyNutritionLogErrors
    {
        public static Error NotFound(Guid id) =>
            new("DailyNutritionLog.NotFound", $"Daily nutrition log with ID '{id}' was not found.");

        public static Error AlreadyExistsForDate(DateTime date) =>
            new("DailyNutritionLog.AlreadyExistsForDate", $"A daily nutrition log already exists for date '{date:yyyy-MM-dd}'.");

        public static Error EntryNotFound(Guid logId, Guid entryId) =>
            new("DailyNutritionLog.EntryNotFound", $"Log entry with ID '{entryId}' was not found in log '{logId}'.");
    }
}
