using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.Foods.UpdateFood
{
    public sealed class UpdateFoodEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/foods/{id:guid}", async (Guid id, UpdateFoodRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateFoodCommand(
                    id,
                    request.Name,
                    request.Category,
                    request.DefaultServingSize,
                    request.ServingUnit,
                    request.Calories,
                    request.Protein,
                    request.Carbohydrates,
                    request.Fat,
                    request.Fiber);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update food failed.");
            })
            .WithName("UpdateFood")
            .WithTags("Foods")
            .WithSummary("Updates an existing food")
            .WithDescription("Updates the food with the specified ID")
            .Accepts<UpdateFoodRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record UpdateFoodRequest(
            string Name,
            string Category,
            decimal DefaultServingSize,
            string ServingUnit,
            decimal Calories,
            decimal Protein,
            decimal Carbohydrates,
            decimal Fat,
            decimal? Fiber);
    }
}
