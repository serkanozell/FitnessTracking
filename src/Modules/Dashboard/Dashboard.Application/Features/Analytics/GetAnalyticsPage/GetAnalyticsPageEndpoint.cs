using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dashboard.Application.Features.Analytics.GetAnalyticsPage
{
    public sealed class GetAnalyticsPageEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard/analytics/page", async (int? days,
                                                                 GroupingPeriodDto? period,
                                                                 Guid? exerciseId,
                                                                 Guid? programId,
                                                                 Guid? splitId,
                                                                 ISender sender,
                                                                 CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAnalyticsPageQuery(days ?? 30,
                                                                         period ?? GroupingPeriodDto.Day,
                                                                         exerciseId,
                                                                         programId,
                                                                         splitId), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load analytics page.");
            })
            .WithName("GetAnalyticsPage")
            .WithTags("Dashboard")
            .WithSummary("Gets all analytics page data in a single response")
            .WithDescription("Aggregates exercises, programs, volume trend, muscle group distribution, personal records and selected exercise progress for the analytics page")
            .Produces<AnalyticsPageDto>(StatusCodes.Status200OK)
            .RequireRateLimiting(RateLimitPolicies.Dashboard);
        }
    }
}
