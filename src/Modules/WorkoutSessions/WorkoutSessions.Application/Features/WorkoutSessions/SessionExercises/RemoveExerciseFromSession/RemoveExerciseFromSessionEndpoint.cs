using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.RemoveExerciseFromSession;

public sealed class RemoveExerciseFromSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/workout-sessions/{sessionId:guid}/exercises/{sessionExerciseId:guid}", async (
            Guid sessionId,
            Guid sessionExerciseId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveExerciseFromSessionCommand(sessionId, sessionExerciseId), ct);

            return result.IsSuccess
                ? Results.NoContent()
                : result.Error!.ToProblem("Remove exercise failed.");
        })
        .WithName("RemoveExerciseFromSession")
        .WithTags("SessionExercises")
        .WithSummary("Removes an exercise entry from a session")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}