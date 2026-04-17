namespace Nutrition.Contracts;

public interface INutritionModule
{
    Task<DailyNutritionSummary?> GetDailySummaryAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
}
