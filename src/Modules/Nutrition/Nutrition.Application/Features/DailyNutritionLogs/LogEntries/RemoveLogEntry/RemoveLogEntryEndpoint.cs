using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.RemoveLogEntry
{
    public sealed class RemoveLogEntryEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/daily-nutrition-logs/{logId:guid}/entries/{entryId:guid}", async (
                Guid logId, Guid entryId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RemoveLogEntryCommand(logId, entryId), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : result.Error!.ToProblem("Remove log entry failed.");
            })
            .WithName("RemoveLogEntry")
            .WithTags("DailyNutritionLogEntries")
            .WithSummary("Removes a food entry from a daily nutrition log")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
