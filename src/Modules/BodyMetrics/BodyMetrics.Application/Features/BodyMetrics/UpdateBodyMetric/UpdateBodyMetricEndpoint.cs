using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BodyMetrics.Application.Features.BodyMetrics.UpdateBodyMetric
{
    public sealed class UpdateBodyMetricEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/body-metrics/{id:guid}", async (Guid id, UpdateBodyMetricRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateBodyMetricCommand(id,
                                                          request.Date,
                                                          request.Weight,
                                                          request.Height,
                                                          request.BodyFatPercentage,
                                                          request.MuscleMass,
                                                          request.WaistCircumference,
                                                          request.ChestCircumference,
                                                          request.ArmCircumference,
                                                          request.HipCircumference,
                                                          request.ThighCircumference,
                                                          request.NeckCircumference,
                                                          request.ShoulderCircumference,
                                                          request.Note);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update body metric failed.");
            })
            .WithName("UpdateBodyMetric")
            .WithTags("BodyMetrics")
            .WithSummary("Updates a body metric entry")
            .WithDescription("Updates body measurements for the specified entry")
            .Accepts<UpdateBodyMetricRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record UpdateBodyMetricRequest(DateTime Date,
                                                     decimal? Weight,
                                                     decimal? Height,
                                                     decimal? BodyFatPercentage,
                                                     decimal? MuscleMass,
                                                     decimal? WaistCircumference,
                                                     decimal? ChestCircumference,
                                                     decimal? ArmCircumference,
                                                     decimal? HipCircumference,
                                                     decimal? ThighCircumference,
                                                     decimal? NeckCircumference,
                                                     decimal? ShoulderCircumference,
                                                     string? Note);
    }
}