using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.Meals.RemoveMeal
{
    public sealed class RemoveMealEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/meal-plans/{mealPlanId:guid}/meals/{mealId:guid}", async (
                Guid mealPlanId, Guid mealId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RemoveMealCommand(mealPlanId, mealId), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Remove meal failed.");
            })
            .WithName("RemoveMeal")
            .WithTags("MealPlanMeals")
            .WithSummary("Removes a meal from a meal plan")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
