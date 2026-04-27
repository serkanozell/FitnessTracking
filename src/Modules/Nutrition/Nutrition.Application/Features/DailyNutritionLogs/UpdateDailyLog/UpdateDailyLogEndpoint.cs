using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.DailyNutritionLogs.UpdateDailyLog
{
    public sealed class UpdateDailyLogEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/daily-nutrition-logs/{id:guid}", async (Guid id, UpdateDailyLogRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateDailyLogCommand(id, request.DailyCalorieGoal, request.Note);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update daily nutrition log failed.");
            })
            .WithName("UpdateDailyNutritionLog")
            .WithTags("DailyNutritionLogs")
            .WithSummary("Updates a daily nutrition log")
            .WithDescription("Updates the calorie goal and note of a daily nutrition log")
            .Accepts<UpdateDailyLogRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record UpdateDailyLogRequest(decimal? DailyCalorieGoal, string? Note);
    }
}
