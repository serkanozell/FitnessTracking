using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionById;

public sealed class GetWorkoutSessionByIdEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/workoutsessions/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkoutSessionByIdQuery(id), ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.Problem(title: "Session not found.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("GetWorkoutSessionById")
        .WithTags("WorkoutSessions")
        .WithSummary("Gets a workout session by ID")
        .Produces<WorkoutSessionDetailDto>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}