using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Features.WorkoutSessions.UpdateWorkoutSession;

public sealed class UpdateWorkoutSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/workout-sessions/{sessionId:guid}", async (Guid sessionId, UpdateSessionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateWorkoutSessionCommand(sessionId, request.Date);
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : result.Error!.ToProblem("Update session failed.");
        })
        .WithName("UpdateWorkoutSession")
        .WithTags("WorkoutSessions")
        .WithSummary("Updates a workout session")
        .Accepts<UpdateSessionRequest>("application/json")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record UpdateSessionRequest(DateTime Date);
}