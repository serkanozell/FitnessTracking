using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram;

public sealed class GetWorkoutSessionsByProgramEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/workoutsessions/byprogram/{programId:guid}", async (
            Guid programId,
            int? pageNumber,
            int? pageSize,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetWorkoutSessionsByProgramQuery(
                programId,
                pageNumber ?? PaginationDefaults.DefaultPageNumber,
                pageSize ?? PaginationDefaults.DefaultPageSize);

            var result = await sender.Send(query, ct);

            return result.IsSuccess
                ? Results.Ok(result.Data)
                : Results.Problem(title: "Failed to retrieve sessions.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
        })
        .WithName("GetWorkoutSessionsByProgram")
        .WithTags("WorkoutSessions")
        .WithSummary("Gets sessions for a workout program with pagination")
        .WithDescription("Returns a paginated list of workout sessions filtered by program ID")
        .Produces<PagedResult<WorkoutSessionDto>>(StatusCodes.Status200OK);
    }
}