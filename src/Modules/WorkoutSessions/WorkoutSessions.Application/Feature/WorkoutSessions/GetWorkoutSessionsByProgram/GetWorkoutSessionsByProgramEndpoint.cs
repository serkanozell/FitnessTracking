using BuildingBlocks.Web;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram;

public sealed class GetWorkoutSessionsByProgramEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/workoutsessions/by-program/{programId:guid}", async (
            Guid programId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetWorkoutSessionsByProgramQuery(programId), ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.Problem(title: "Failed to retrieve sessions.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
        })
        .WithName("GetWorkoutSessionsByProgram")
        .WithTags("WorkoutSessions")
        .WithSummary("Gets all sessions for a workout program")
        .WithDescription("Returns a list of workout sessions filtered by program ID")
        .Produces<IReadOnlyList<WorkoutSessionDto>>(StatusCodes.Status200OK);
    }
}