using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.Foods.DeleteFood
{
    public sealed class DeleteFoodEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/foods/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteFoodCommand(id), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Delete food failed.");
            })
            .WithName("DeleteFood")
            .WithTags("Foods")
            .WithSummary("Deletes a food")
            .WithDescription("Soft deletes the food with the specified ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
