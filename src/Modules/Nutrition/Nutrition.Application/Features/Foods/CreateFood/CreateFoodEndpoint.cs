using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.Foods.CreateFood
{
    public sealed class CreateFoodEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/foods", async (CreateFoodRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateFoodCommand(
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
                    ? Results.Created($"/api/v1/foods/{result.Data}", new CreateFoodResponse(result.Data))
                    : result.Error!.ToProblem("Create food failed.");
            })
            .WithName("CreateFood")
            .WithTags("Foods")
            .WithSummary("Creates a new food")
            .WithDescription("Creates a new food entry with nutritional information for the authenticated user")
            .Accepts<CreateFoodRequest>("application/json")
            .Produces<CreateFoodResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record CreateFoodRequest(
            string Name,
            string Category,
            decimal DefaultServingSize,
            string ServingUnit,
            decimal Calories,
            decimal Protein,
            decimal Carbohydrates,
            decimal Fat,
            decimal? Fiber);

        public sealed record CreateFoodResponse(Guid Id);
    }
}
