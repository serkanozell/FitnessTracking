using BuildingBlocks.Application.Pagination;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.MealPlans.GetMealPlans
{
    public sealed record GetMealPlansQuery(
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<MealPlanDto>>>;
}
