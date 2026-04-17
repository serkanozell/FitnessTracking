using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.Meals.AddMeal
{
    public sealed class AddMealEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/meal-plans/{mealPlanId:guid}/meals", async (
                Guid mealPlanId, AddMealRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new AddMealCommand(mealPlanId, request.Name, request.Order);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/meal-plans/{mealPlanId}", new AddMealResponse(result.Data))
                    : result.Error!.ToProblem("Add meal failed.");
            })
            .WithName("AddMeal")
            .WithTags("MealPlanMeals")
            .WithSummary("Adds a meal to a meal plan")
            .Accepts<AddMealRequest>("application/json")
            .Produces<AddMealResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record AddMealRequest(string Name, int Order);
        public sealed record AddMealResponse(Guid MealId);
    }
}
