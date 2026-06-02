namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.AddLogEntry
{
    public sealed record AddLogEntryCommand(
        Guid DailyNutritionLogId,
        Guid FoodId,
        decimal Quantity,
        string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}
