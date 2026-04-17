using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.MealPlans.CreateMealPlan
{
    public sealed class CreateMealPlanEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/meal-plans", async (CreateMealPlanRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateMealPlanCommand(request.Name, request.Date, request.Note);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/meal-plans/{result.Data}", new CreateMealPlanResponse(result.Data))
                    : result.Error!.ToProblem("Create meal plan failed.");
            })
            .WithName("CreateMealPlan")
            .WithTags("MealPlans")
            .WithSummary("Creates a new meal plan")
            .WithDescription("Creates a new meal plan for the authenticated user")
            .Accepts<CreateMealPlanRequest>("application/json")
            .Produces<CreateMealPlanResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record CreateMealPlanRequest(string Name, DateTime Date, string? Note);
        public sealed record CreateMealPlanResponse(Guid Id);
    }
}
