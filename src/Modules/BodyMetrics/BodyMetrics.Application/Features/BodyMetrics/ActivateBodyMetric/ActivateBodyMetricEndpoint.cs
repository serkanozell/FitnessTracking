using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BodyMetrics.Application.Features.BodyMetrics.ActivateBodyMetric
{
    public sealed class ActivateBodyMetricEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/body-metrics/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ActivateBodyMetricCommand(id), ct);

                return result.IsSuccess
                    ? Results.Ok(new ActivateBodyMetricResponse(result.Data))
                    : result.Error!.ToProblem("Activate body metric failed.");
            })
            .WithName("ActivateBodyMetric")
            .WithTags("BodyMetrics")
            .WithSummary("Activates a body metric entry")
            .WithDescription("Reactivates a previously deleted body metric entry")
            .Produces<ActivateBodyMetricResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record ActivateBodyMetricResponse(Guid Id);
    }
}