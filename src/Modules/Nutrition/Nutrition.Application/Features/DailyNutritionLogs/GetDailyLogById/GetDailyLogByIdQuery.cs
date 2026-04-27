using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.DailyNutritionLogs.GetDailyLogById
{
    public sealed record GetDailyLogByIdQuery(Guid Id) : IQuery<Result<DailyNutritionLogDto>>;
}
