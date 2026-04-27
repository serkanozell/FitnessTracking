namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.AddLogEntry
{
    public sealed record AddLogEntryCommand(
        Guid DailyNutritionLogId,
        Guid FoodId,
        decimal Quantity) : ICommand<Result<Guid>>;
}
