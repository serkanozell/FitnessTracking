using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.RemoveMealItem
{
    public sealed class RemoveMealItemEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/meal-plans/{mealPlanId:guid}/meals/{mealId:guid}/items/{mealItemId:guid}", async (
                Guid mealPlanId, Guid mealId, Guid mealItemId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RemoveMealItemCommand(mealPlanId, mealId, mealItemId), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Remove meal item failed.");
            })
            .WithName("RemoveMealItem")
            .WithTags("MealPlanMealItems")
            .WithSummary("Removes a food item from a meal")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
