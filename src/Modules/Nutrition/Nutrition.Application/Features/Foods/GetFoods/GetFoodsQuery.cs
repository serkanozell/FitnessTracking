using BuildingBlocks.Application.Pagination;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.Foods.GetFoods
{
    public sealed record GetFoodsQuery(
        int PageNumber = PaginationDefaults.DefaultPageNumber,
        int PageSize = PaginationDefaults.DefaultPageSize) : IQuery<Result<PagedResult<FoodDto>>>;
}
