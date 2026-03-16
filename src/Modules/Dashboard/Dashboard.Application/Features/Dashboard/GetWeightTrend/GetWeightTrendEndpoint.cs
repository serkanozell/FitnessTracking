using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dashboard.Application.Features.Dashboard.GetWeightTrend
{
    public sealed class GetWeightTrendEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard/weight-trend", async (int? days, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetWeightTrendQuery(days ?? 90), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load weight trend.");
            })
            .WithName("GetWeightTrend")
            .WithTags("Dashboard")
            .WithSummary("Gets weight trend for the current user")
            .WithDescription("Returns weight measurements over the specified period (default 90 days)")
            .Produces<IReadOnlyList<WeightTrendDto>>(StatusCodes.Status200OK);
        }
    }
}