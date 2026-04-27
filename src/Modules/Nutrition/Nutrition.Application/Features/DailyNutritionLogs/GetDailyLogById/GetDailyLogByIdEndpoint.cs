using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.DailyNutritionLogs.GetDailyLogById
{
    public sealed class GetDailyLogByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/daily-nutrition-logs/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetDailyLogByIdQuery(id), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Daily nutrition log not found.");
            })
            .WithName("GetDailyNutritionLogById")
            .WithTags("DailyNutritionLogs")
            .WithSummary("Gets a daily nutrition log by ID")
            .WithDescription("Returns a daily nutrition log with all entries and calculated macros")
            .Produces<DailyNutritionLogDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
