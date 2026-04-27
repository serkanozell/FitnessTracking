namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.RemoveLogEntry
{
    public sealed record RemoveLogEntryCommand(
        Guid DailyNutritionLogId,
        Guid EntryId) : ICommand<Result<bool>>;
}
