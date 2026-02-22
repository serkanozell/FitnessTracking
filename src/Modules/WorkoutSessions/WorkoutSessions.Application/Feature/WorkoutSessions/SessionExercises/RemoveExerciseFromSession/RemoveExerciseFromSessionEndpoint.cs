using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession;

public sealed class RemoveExerciseFromSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/workoutsessions/{sessionId:guid}/exercises/{exerciseId:guid}", async (
            Guid sessionId,
            Guid exerciseId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new RemoveExerciseFromSessionCommand(sessionId, exerciseId), ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(title: "Remove exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("RemoveExerciseFromSession")
        .WithTags("SessionExercises")
        .WithSummary("Removes an exercise entry from a session")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}