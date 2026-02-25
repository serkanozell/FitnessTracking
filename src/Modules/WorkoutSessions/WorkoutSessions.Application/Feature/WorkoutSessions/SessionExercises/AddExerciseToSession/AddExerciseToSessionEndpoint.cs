using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession;

public sealed class AddExerciseToSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/workout-sessions/{sessionId:guid}/exercises", async (
            Guid sessionId,
            AddExerciseRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddExerciseToSessionCommand(
                sessionId,
                request.ExerciseId,
                request.SetNumber,
                request.Weight,
                request.Reps);

            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/workout-sessions/{sessionId}/exercises/{result.Data}", new AddExerciseResponse(result.Data))
                : Results.Problem(title: "Add exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
        })
        .WithName("AddExerciseToSession")
        .WithTags("SessionExercises")
        .WithSummary("Adds an exercise entry to a session")
        .Accepts<AddExerciseRequest>("application/json")
        .Produces<AddExerciseResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record AddExerciseRequest(Guid ExerciseId, int SetNumber, decimal Weight, int Reps);
    public sealed record AddExerciseResponse(Guid SessionExerciseId);
}