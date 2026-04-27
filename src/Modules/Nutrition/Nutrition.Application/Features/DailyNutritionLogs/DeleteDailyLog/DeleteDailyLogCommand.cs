namespace Nutrition.Application.Features.DailyNutritionLogs.DeleteDailyLog
{
    public sealed record DeleteDailyLogCommand(Guid Id) : ICommand<Result<bool>>;
}
