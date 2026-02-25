using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession;

public sealed class DeleteWorkoutSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/workout-sessions/{sessionId:guid}", async (Guid sessionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteWorkoutSessionCommand(sessionId), ct);

            return result.IsSuccess
                ? Results.NoContent()
                : Results.Problem(title: "Delete session failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("DeleteWorkoutSession")
        .WithTags("WorkoutSessions")
        .WithSummary("Deletes a workout session")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}