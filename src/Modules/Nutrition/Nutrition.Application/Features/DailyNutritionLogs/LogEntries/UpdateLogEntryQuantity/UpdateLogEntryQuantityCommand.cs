namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.UpdateLogEntryQuantity
{
    public sealed record UpdateLogEntryQuantityCommand(
        Guid DailyNutritionLogId,
        Guid EntryId,
        decimal Quantity) : ICommand<Result<bool>>;
}
