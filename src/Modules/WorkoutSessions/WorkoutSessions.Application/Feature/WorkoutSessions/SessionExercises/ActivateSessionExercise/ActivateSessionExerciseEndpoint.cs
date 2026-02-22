using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.ActivateSessionExercise;

public sealed class ActivateSessionExerciseEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/workoutsessions/{sessionId:guid}/exercises/{exerciseId:guid}/activate", async (
            Guid sessionId,
            Guid exerciseId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new ActivateSessionExerciseCommand(sessionId, exerciseId), ct);

            return result.IsSuccess
                ? Results.Ok(new ActivateExerciseResponse(result.Data))
                : Results.Problem(title: "Activate exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("ActivateSessionExercise")
        .WithTags("SessionExercises")
        .WithSummary("Activates an exercise entry in a session")
        .Produces<ActivateExerciseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record ActivateExerciseResponse(Guid SessionExerciseId);
}