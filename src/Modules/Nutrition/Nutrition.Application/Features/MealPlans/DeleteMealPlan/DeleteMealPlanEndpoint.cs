using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.DeleteMealPlan
{
    public sealed class DeleteMealPlanEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/meal-plans/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteMealPlanCommand(id), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Delete meal plan failed.");
            })
            .WithName("DeleteMealPlan")
            .WithTags("MealPlans")
            .WithSummary("Deletes a meal plan")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
