using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dashboard.Application.Features.Analytics.GetMuscleGroupDistribution
{
    public sealed class GetMuscleGroupDistributionEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard/analytics/muscle-group-distribution", async (int? days, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetMuscleGroupDistributionQuery(days ?? 30), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load muscle group distribution.");
            })
            .WithName("GetMuscleGroupDistribution")
            .WithTags("Dashboard")
            .WithSummary("Gets training volume distribution by primary muscle group")
            .WithDescription("Returns total volume, sets and reps grouped by primary muscle group over the period (default 30 days)")
            .Produces<IReadOnlyList<MuscleGroupVolumeDto>>(StatusCodes.Status200OK);
        }
    }
}
