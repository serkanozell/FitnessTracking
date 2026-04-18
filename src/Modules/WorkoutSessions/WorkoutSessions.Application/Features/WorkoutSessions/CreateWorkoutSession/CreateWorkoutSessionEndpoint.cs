using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession;

public sealed class CreateWorkoutSessionEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/workout-sessions", async (CreateSessionRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new CreateWorkoutSessionCommand(request.WorkoutProgramId, request.WorkoutProgramSplitId, request.Date);
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.Created($"/api/v1/workout-sessions/{result.Data}", new CreateSessionResponse(result.Data))
                : result.Error!.ToProblem("Create session failed.");
        })
        .WithName("CreateWorkoutSession")
        .WithTags("WorkoutSessions")
        .WithSummary("Creates a new workout session")
        .Accepts<CreateSessionRequest>("application/json")
        .Produces<CreateSessionResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public sealed record CreateSessionRequest(Guid WorkoutProgramId, Guid WorkoutProgramSplitId, DateTime Date);
    public sealed record CreateSessionResponse(Guid Id);
}