using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.MealPlans.GetMealPlans
{
    public sealed class GetMealPlansEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/meal-plans", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetMealPlansQuery(
                    pageNumber ?? PaginationDefaults.DefaultPageNumber,
                    pageSize ?? PaginationDefaults.DefaultPageSize);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to retrieve meal plans.");
            })
            .WithName("GetMealPlans")
            .WithTags("MealPlans")
            .WithSummary("Gets meal plans with pagination")
            .WithDescription("Returns a paginated list of meal plans for the authenticated user")
            .Produces<PagedResult<MealPlanDto>>(StatusCodes.Status200OK);
        }
    }
}
