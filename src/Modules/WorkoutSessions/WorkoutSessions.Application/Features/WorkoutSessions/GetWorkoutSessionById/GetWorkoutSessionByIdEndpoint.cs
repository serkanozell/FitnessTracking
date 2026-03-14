using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionById;

public sealed class GetWorkoutSessionByIdEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/workout-sessions/{sessionId:guid}", async (Guid sessionId, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkoutSessionByIdQuery(sessionId), ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : result.Error!.ToProblem("Session not found.");
        })
        .WithName("GetWorkoutSessionById")
        .WithTags("WorkoutSessions")
        .WithSummary("Gets a workout session by ID")
        .Produces<WorkoutSessionDetailDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}