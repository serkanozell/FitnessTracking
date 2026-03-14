using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.ActivateSessionExercise;

public sealed class ActivateSessionExerciseEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/workout-sessions/{sessionId:guid}/exercises/{sessionExerciseId:guid}/activate", async (
            Guid sessionId,
            Guid sessionExerciseId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new ActivateSessionExerciseCommand(sessionId, sessionExerciseId), ct);

            return result.IsSuccess
                ? Results.Ok(new ActivateExerciseResponse(result.Data))
                : result.Error!.ToProblem("Activate exercise failed.");
        })
        .WithName("ActivateSessionExercise")
        .WithTags("SessionExercises")
        .WithSummary("Activates an exercise entry in a session")
        .Produces<ActivateExerciseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record ActivateExerciseResponse(Guid SessionExerciseId);
}