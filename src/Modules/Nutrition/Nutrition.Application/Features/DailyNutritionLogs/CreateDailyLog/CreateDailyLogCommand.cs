namespace Nutrition.Application.Features.DailyNutritionLogs.CreateDailyLog
{
    public sealed record CreateDailyLogCommand(
        DateTime Date,
        decimal? DailyCalorieGoal,
        string? Note) : ICommand<Result<Guid>>;
}
