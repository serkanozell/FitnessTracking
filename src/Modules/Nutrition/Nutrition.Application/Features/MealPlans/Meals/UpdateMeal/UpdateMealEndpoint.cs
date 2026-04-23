using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.Meals.UpdateMeal
{
    public sealed class UpdateMealEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/meal-plans/{mealPlanId:guid}/meals/{mealId:guid}", async (
                Guid mealPlanId, Guid mealId, UpdateMealRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateMealCommand(mealPlanId, mealId, request.Name, request.Order);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update meal failed.");
            })
            .WithName("UpdateMeal")
            .WithTags("MealPlanMeals")
            .WithSummary("Updates a meal in a meal plan")
            .Accepts<UpdateMealRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record UpdateMealRequest(string Name, int Order);
    }
}
