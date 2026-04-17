using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.UpdateMealPlan
{
    public sealed class UpdateMealPlanEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/meal-plans/{id:guid}", async (Guid id, UpdateMealPlanRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateMealPlanCommand(id, request.Name, request.Date, request.Note);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update meal plan failed.");
            })
            .WithName("UpdateMealPlan")
            .WithTags("MealPlans")
            .WithSummary("Updates an existing meal plan")
            .Accepts<UpdateMealPlanRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record UpdateMealPlanRequest(string Name, DateTime Date, string? Note);
    }
}
