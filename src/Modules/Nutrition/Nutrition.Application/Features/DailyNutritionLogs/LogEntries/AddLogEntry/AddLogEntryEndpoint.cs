using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.AddLogEntry
{
    public sealed class AddLogEntryEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/daily-nutrition-logs/{logId:guid}/entries", async (
                Guid logId, AddLogEntryRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new AddLogEntryCommand(logId, request.FoodId, request.Quantity);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/daily-nutrition-logs/{logId}", new AddLogEntryResponse(result.Data))
                    : result.Error!.ToProblem("Add log entry failed.");
            })
            .WithName("AddLogEntry")
            .WithTags("DailyNutritionLogEntries")
            .WithSummary("Adds a food entry to a daily nutrition log")
            .WithDescription("Adds a food to the specified daily nutrition log. Macros are calculated automatically from the food's nutritional data.")
            .Accepts<AddLogEntryRequest>("application/json")
            .Produces<AddLogEntryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record AddLogEntryRequest(Guid FoodId, decimal Quantity);
        public sealed record AddLogEntryResponse(Guid EntryId);
    }
}
