using BuildingBlocks.Application.Pagination;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.DailyNutritionLogs.GetDailyLogs
{
    public sealed record GetDailyLogsQuery(
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<DailyNutritionLogDto>>>;
}
