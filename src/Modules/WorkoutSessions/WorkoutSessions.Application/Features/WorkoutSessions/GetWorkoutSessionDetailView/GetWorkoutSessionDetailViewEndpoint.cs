using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionDetailView;

public sealed class GetWorkoutSessionDetailViewEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/workout-sessions/{sessionId:guid}/detail-view",
            async (Guid sessionId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetWorkoutSessionDetailViewQuery(sessionId), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Session not found.");
            })
            .WithName("GetWorkoutSessionDetailView")
            .WithTags("WorkoutSessions")
            .WithSummary("Gets the aggregated detail view for a workout session in a single call")
            .Produces<WorkoutSessionDetailViewDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
