namespace Nutrition.Application.Features.DailyNutritionLogs.UpdateDailyLog
{
    public sealed record UpdateDailyLogCommand(
        Guid Id,
        decimal? DailyCalorieGoal,
        string? Note) : ICommand<Result<bool>>;
}
