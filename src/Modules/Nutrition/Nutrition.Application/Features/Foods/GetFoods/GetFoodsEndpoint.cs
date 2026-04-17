using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.Foods.GetFoods
{
    public sealed class GetFoodsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/foods", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetFoodsQuery(
                    pageNumber ?? PaginationDefaults.DefaultPageNumber,
                    pageSize ?? PaginationDefaults.DefaultPageSize);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to retrieve foods.");
            })
            .WithName("GetFoods")
            .WithTags("Foods")
            .WithSummary("Gets foods with pagination")
            .WithDescription("Returns a paginated list of system foods and user's custom foods")
            .Produces<PagedResult<FoodDto>>(StatusCodes.Status200OK);
        }
    }
}
