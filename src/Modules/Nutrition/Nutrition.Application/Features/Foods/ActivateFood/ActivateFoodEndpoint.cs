using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.Foods.ActivateFood
{
    public sealed class ActivateFoodEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/foods/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ActivateFoodCommand(id), ct);

                return result.IsSuccess
                    ? Results.Ok(new ActivateFoodResponse(result.Data))
                    : result.Error!.ToProblem("Activation failed.");
            })
            .WithName("ActivateFood")
            .WithTags("Foods")
            .WithSummary("Activates a food")
            .WithDescription("Activates a previously deactivated food")
            .Produces<ActivateFoodResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record ActivateFoodResponse(Guid Id);
    }
}
