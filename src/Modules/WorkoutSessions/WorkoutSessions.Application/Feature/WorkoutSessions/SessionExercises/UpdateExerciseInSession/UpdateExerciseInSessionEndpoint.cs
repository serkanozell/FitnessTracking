using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession;

public sealed class UpdateExerciseInSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/workoutsessions/{sessionId:guid}/exercises/{exerciseId:guid}", async (
            Guid sessionId,
            Guid exerciseId,
            UpdateExerciseRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateExerciseInSessionCommand(
                sessionId,
                exerciseId,
                request.SetNumber,
                request.Weight,
                request.Reps);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(title: "Update exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("UpdateExerciseInSession")
        .WithTags("SessionExercises")
        .WithSummary("Updates an exercise entry in a session")
        .Accepts<UpdateExerciseRequest>("application/json")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record UpdateExerciseRequest(int SetNumber, decimal Weight, int Reps);
}