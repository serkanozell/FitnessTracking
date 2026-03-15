using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BodyMetrics.Application.Features.BodyMetrics.DeleteBodyMetric
{
    public sealed class DeleteBodyMetricEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/body-metrics/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteBodyMetricCommand(id), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Delete body metric failed.");
            })
            .WithName("DeleteBodyMetric")
            .WithTags("BodyMetrics")
            .WithSummary("Deletes a body metric entry")
            .WithDescription("Soft deletes the body metric with the specified ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}