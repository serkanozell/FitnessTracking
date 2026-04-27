using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.UpdateLogEntryQuantity
{
    public sealed class UpdateLogEntryQuantityEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/daily-nutrition-logs/{logId:guid}/entries/{entryId:guid}/quantity", async (
                Guid logId, Guid entryId, UpdateLogEntryQuantityRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateLogEntryQuantityCommand(logId, entryId, request.Quantity);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Update log entry quantity failed.");
            })
            .WithName("UpdateLogEntryQuantity")
            .WithTags("DailyNutritionLogEntries")
            .WithSummary("Updates the quantity of a log entry")
            .WithDescription("Updates the quantity and recalculates macros for a food entry in a daily nutrition log")
            .Accepts<UpdateLogEntryQuantityRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record UpdateLogEntryQuantityRequest(decimal Quantity);
    }
}
