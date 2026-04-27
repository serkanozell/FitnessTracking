using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.DailyNutritionLogs.DeleteDailyLog
{
    public sealed class DeleteDailyLogEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/daily-nutrition-logs/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteDailyLogCommand(id), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Delete daily nutrition log failed.");
            })
            .WithName("DeleteDailyNutritionLog")
            .WithTags("DailyNutritionLogs")
            .WithSummary("Deletes a daily nutrition log")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
