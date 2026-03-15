using BuildingBlocks.Web;
using BodyMetrics.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BodyMetrics.Application.Features.BodyMetrics.GetBodyMetricById
{
    public sealed class GetBodyMetricByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/body-metrics/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetBodyMetricByIdQuery(id), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Body metric not found.");
            })
            .WithName("GetBodyMetricById")
            .WithTags("BodyMetrics")
            .WithSummary("Gets a body metric by ID")
            .WithDescription("Returns the body metric entry with the specified ID")
            .Produces<BodyMetricDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}