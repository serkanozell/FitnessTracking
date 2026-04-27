using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.DailyNutritionLogs.GetDailyLogs
{
    public sealed class GetDailyLogsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/daily-nutrition-logs", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetDailyLogsQuery(
                    pageNumber ?? PaginationDefaults.DefaultPageNumber,
                    pageSize ?? PaginationDefaults.DefaultPageSize);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to retrieve daily nutrition logs.");
            })
            .WithName("GetDailyNutritionLogs")
            .WithTags("DailyNutritionLogs")
            .WithSummary("Gets daily nutrition logs with pagination")
            .WithDescription("Returns a paginated list of daily nutrition logs for the authenticated user")
            .Produces<PagedResult<DailyNutritionLogDto>>(StatusCodes.Status200OK);
        }
    }
}
