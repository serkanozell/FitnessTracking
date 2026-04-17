using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.ActivateMealPlan
{
    public sealed class ActivateMealPlanEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/meal-plans/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ActivateMealPlanCommand(id), ct);

                return result.IsSuccess
                    ? Results.Ok(new ActivateMealPlanResponse(result.Data))
                    : result.Error!.ToProblem("Activation failed.");
            })
            .WithName("ActivateMealPlan")
            .WithTags("MealPlans")
            .WithSummary("Activates a meal plan")
            .Produces<ActivateMealPlanResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record ActivateMealPlanResponse(Guid Id);
    }
}
