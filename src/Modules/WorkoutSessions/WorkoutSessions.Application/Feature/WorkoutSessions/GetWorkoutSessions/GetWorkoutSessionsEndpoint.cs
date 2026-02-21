using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions;

public sealed class GetWorkoutSessionsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/workoutsessions", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkoutSessionsQuery(), ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.Problem(title: "Failed to retrieve sessions.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
        })
        .WithName("GetWorkoutSessions")
        .WithTags("WorkoutSessions")
        .WithSummary("Gets all workout sessions")
        .Produces<IReadOnlyList<WorkoutSessionDto>>(StatusCodes.Status200OK);
    }
}