using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.UpdateMealItemQuantity
{
    public sealed class UpdateMealItemQuantityEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/meal-plans/{mealPlanId:guid}/meals/{mealId:guid}/items/{mealItemId:guid}", async (
                Guid mealPlanId, Guid mealId, Guid mealItemId,
                UpdateMealItemQuantityRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateMealItemQuantityCommand(mealPlanId, mealId, mealItemId, request.Quantity);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update quantity failed.");
            })
            .WithName("UpdateMealItemQuantity")
            .WithTags("MealPlanMealItems")
            .WithSummary("Updates a meal item's quantity")
            .WithDescription("Updates the quantity and recalculates macros for the specified meal item")
            .Accepts<UpdateMealItemQuantityRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record UpdateMealItemQuantityRequest(decimal Quantity);
    }
}
