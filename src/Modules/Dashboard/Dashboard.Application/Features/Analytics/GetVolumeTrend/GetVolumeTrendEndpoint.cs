using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Contracts;

namespace Dashboard.Application.Features.Analytics.GetVolumeTrend
{
    public sealed class GetVolumeTrendEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard/analytics/volume-trend", async (int? days, GroupingPeriod? period, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetVolumeTrendQuery(days ?? 30, period ?? GroupingPeriod.Day), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load volume trend.");
            })
            .WithName("GetVolumeTrend")
            .WithTags("Dashboard")
            .WithSummary("Gets workout volume trend (sets × reps × weight) for the current user")
            .WithDescription("Returns aggregated volume per day/week/month over the specified period (default 30 days)")
            .Produces<IReadOnlyList<VolumeTrendPointDto>>(StatusCodes.Status200OK);
        }
    }
}
