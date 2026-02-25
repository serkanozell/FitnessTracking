using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession;

public sealed class ActivateWorkoutSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/workout-sessions/{sessionId:guid}/activate", async (Guid sessionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new ActivateWorkoutSessionCommand(sessionId), ct);

            return result.IsSuccess
                ? Results.Ok(new ActivateSessionResponse(result.Data))
                : Results.Problem(title: "Activation failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("ActivateWorkoutSession")
        .WithTags("WorkoutSessions")
        .WithSummary("Activates a workout session")
        .Produces<ActivateSessionResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record ActivateSessionResponse(Guid Id);
}