using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.GetExercisesBySession;

public sealed class GetExercisesBySessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/workoutsessions/{sessionId:guid}/exercises", async (
            Guid sessionId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetExercisesBySessionQuery(sessionId), ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.Problem(title: "Failed to retrieve exercises.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("GetExercisesBySession")
        .WithTags("SessionExercises")
        .WithSummary("Gets all exercise entries in a session")
        .Produces<IReadOnlyList<SessionExerciseDto>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}