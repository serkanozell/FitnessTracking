using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession;

public sealed class DeleteWorkoutSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/workoutsessions/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteWorkoutSessionCommand(id), ct);

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