using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.AddMealItem
{
    public sealed class AddMealItemEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/meal-plans/{mealPlanId:guid}/meals/{mealId:guid}/items", async (
                Guid mealPlanId, Guid mealId, AddMealItemRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new AddMealItemCommand(mealPlanId, mealId, request.FoodId, request.Quantity);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/meal-plans/{mealPlanId}", new AddMealItemResponse(result.Data))
                    : result.Error!.ToProblem("Add meal item failed.");
            })
            .WithName("AddMealItem")
            .WithTags("MealPlanMealItems")
            .WithSummary("Adds a food item to a meal")
            .WithDescription("Adds a food to the specified meal. Macros are calculated automatically from the food's nutritional data.")
            .Accepts<AddMealItemRequest>("application/json")
            .Produces<AddMealItemResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record AddMealItemRequest(Guid FoodId, decimal Quantity);
        public sealed record AddMealItemResponse(Guid MealItemId);
    }
}
