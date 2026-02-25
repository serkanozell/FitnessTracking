using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessions;

public sealed class GetWorkoutSessionsEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/workout-sessions", async (Guid? programId, int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
        {
            var query = new GetWorkoutSessionsQuery(
                programId,
                pageNumber ?? PaginationDefaults.DefaultPageNumber,
                pageSize ?? PaginationDefaults.DefaultPageSize);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.Problem(title: "Failed to retrieve sessions.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
        })
        .WithName("GetWorkoutSessions")
        .WithTags("WorkoutSessions")
        .WithSummary("Gets workout sessions with pagination")
        .WithDescription("Returns a paginated list of workout sessions. Optionally filter by programId.")
        .Produces<PagedResult<WorkoutSessionDto>>(StatusCodes.Status200OK);
    }
}