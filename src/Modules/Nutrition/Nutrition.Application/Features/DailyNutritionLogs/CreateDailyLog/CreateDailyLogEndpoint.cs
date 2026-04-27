using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.DailyNutritionLogs.CreateDailyLog
{
    public sealed class CreateDailyLogEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/daily-nutrition-logs", async (CreateDailyLogRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateDailyLogCommand(request.Date, request.DailyCalorieGoal, request.Note);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/daily-nutrition-logs/{result.Data}", new CreateDailyLogResponse(result.Data))
                    : result.Error!.ToProblem("Create daily nutrition log failed.");
            })
            .WithName("CreateDailyNutritionLog")
            .WithTags("DailyNutritionLogs")
            .WithSummary("Creates a new daily nutrition log")
            .WithDescription("Creates a new daily nutrition log for the authenticated user. Only one log per date is allowed.")
            .Accepts<CreateDailyLogRequest>("application/json")
            .Produces<CreateDailyLogResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
        }

        public sealed record CreateDailyLogRequest(DateTime Date, decimal? DailyCalorieGoal, string? Note);
        public sealed record CreateDailyLogResponse(Guid Id);
    }
}
