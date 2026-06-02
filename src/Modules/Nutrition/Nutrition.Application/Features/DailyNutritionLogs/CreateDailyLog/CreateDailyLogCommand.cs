namespace Nutrition.Application.Features.DailyNutritionLogs.CreateDailyLog
{
    public sealed record CreateDailyLogCommand(
        DateTime Date,
        decimal? DailyCalorieGoal,
        string? Note,
        string? IdempotencyKey = null) : ICommand<Result<Guid>>, IIdempotentCommand;
}
