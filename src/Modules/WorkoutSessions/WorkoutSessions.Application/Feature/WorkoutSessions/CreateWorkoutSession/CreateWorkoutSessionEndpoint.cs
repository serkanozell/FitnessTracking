using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession;

public sealed class CreateWorkoutSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/workoutsessions", async (CreateSessionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateWorkoutSessionCommand(request.WorkoutProgramId, request.Date);
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/workout-sessions/{result.Data}", new CreateSessionResponse(result.Data))
                : Results.Problem(title: "Create session failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
        })
        .WithName("CreateWorkoutSession")
        .WithTags("WorkoutSessions")
        .WithSummary("Creates a new workout session")
        .Accepts<CreateSessionRequest>("application/json")
        .Produces<CreateSessionResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public sealed record CreateSessionRequest(Guid WorkoutProgramId, DateTime Date);
    public sealed record CreateSessionResponse(Guid Id);
}