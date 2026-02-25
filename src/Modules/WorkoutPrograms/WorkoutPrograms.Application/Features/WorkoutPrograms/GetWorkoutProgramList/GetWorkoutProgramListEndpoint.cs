using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    public sealed class GetWorkoutProgramListEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/workout-programs", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetWorkoutProgramListQuery(
                    pageNumber ?? PaginationDefaults.DefaultPageNumber,
                    pageSize ?? PaginationDefaults.DefaultPageSize);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Failed to retrieve workout programs.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
            })
            .WithName("GetWorkoutProgramList")
            .WithTags("WorkoutPrograms")
            .WithSummary("Gets workout programs with pagination")
            .WithDescription("Returns a paginated list of workout programs. Use pageNumber and pageSize query parameters.")
            .Produces<PagedResult<WorkoutProgramDto>>(StatusCodes.Status200OK);
        }
    }
}