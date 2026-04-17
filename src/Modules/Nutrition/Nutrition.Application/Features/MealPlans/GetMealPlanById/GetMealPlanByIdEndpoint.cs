using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.MealPlans.GetMealPlanById
{
    public sealed class GetMealPlanByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/meal-plans/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetMealPlanByIdQuery(id), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Meal plan not found.");
            })
            .WithName("GetMealPlanById")
            .WithTags("MealPlans")
            .WithSummary("Gets a meal plan by ID")
            .WithDescription("Returns a meal plan with all meals, items, and calculated macros")
            .Produces<MealPlanDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
