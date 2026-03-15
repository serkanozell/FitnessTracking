using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using BodyMetrics.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BodyMetrics.Application.Features.BodyMetrics.GetBodyMetrics
{
    public sealed class GetBodyMetricsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/body-metrics", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetBodyMetricsQuery(pageNumber ?? 1, pageSize ?? 10), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to retrieve body metrics.");
            })
            .WithName("GetBodyMetrics")
            .WithTags("BodyMetrics")
            .WithSummary("Gets body metrics for the current user")
            .WithDescription("Returns a paginated list of body metric entries for the authenticated user")
            .Produces<PagedResult<BodyMetricDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}