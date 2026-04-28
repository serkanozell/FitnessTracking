using BuildingBlocks.Web;
using Dashboard.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Dashboard.Application.Features.Analytics.GetExerciseProgress
{
    public sealed class GetExerciseProgressEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/dashboard/analytics/exercise-progress/{exerciseId:guid}", async (Guid exerciseId, int? days, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetExerciseProgressQuery(exerciseId, days ?? 90), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to load exercise progress.");
            })
            .WithName("GetExerciseProgress")
            .WithTags("Dashboard")
            .WithSummary("Gets progress (max weight, 1RM, volume) for a single exercise")
            .WithDescription("Returns daily best lift metrics for the specified exercise over the period (default 90 days)")
            .Produces<IReadOnlyList<ExerciseProgressPointDto>>(StatusCodes.Status200OK);
        }
    }
}
