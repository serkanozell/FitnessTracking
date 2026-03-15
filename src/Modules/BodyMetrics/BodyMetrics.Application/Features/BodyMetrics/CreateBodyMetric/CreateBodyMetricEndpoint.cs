using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BodyMetrics.Application.Features.BodyMetrics.CreateBodyMetric
{
    public sealed class CreateBodyMetricEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/body-metrics", async (CreateBodyMetricRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateBodyMetricCommand(
                    request.Date, request.Weight, request.Height,
                    request.BodyFatPercentage, request.MuscleMass,
                    request.WaistCircumference, request.ChestCircumference,
                    request.ArmCircumference, request.HipCircumference,
                    request.ThighCircumference, request.NeckCircumference,
                    request.Note);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/body-metrics/{result.Data}", new CreateBodyMetricResponse(result.Data))
                    : result.Error!.ToProblem("Create body metric failed.");
            })
            .WithName("CreateBodyMetric")
            .WithTags("BodyMetrics")
            .WithSummary("Creates a new body metric entry")
            .WithDescription("Records body measurements for the authenticated user")
            .Accepts<CreateBodyMetricRequest>("application/json")
            .Produces<CreateBodyMetricResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record CreateBodyMetricRequest(
            DateTime Date, decimal? Weight, decimal? Height,
            decimal? BodyFatPercentage, decimal? MuscleMass,
            decimal? WaistCircumference, decimal? ChestCircumference,
            decimal? ArmCircumference, decimal? HipCircumference,
            decimal? ThighCircumference, decimal? NeckCircumference,
            string? Note);

        public sealed record CreateBodyMetricResponse(Guid Id);
    }
}