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
            endpoints.MapGet("/workout-programs", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetWorkoutProgramListQuery(
                    pageNumber ?? PaginationDefaults.DefaultPageNumber,
                    pageSize ?? PaginationDefaults.DefaultPageSize);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Failed to retrieve workout programs.");
            })
            .WithName("GetWorkoutProgramList")
            .WithTags("WorkoutPrograms")
            .WithSummary("Gets workout programs with pagination")
            .WithDescription("Returns a paginated list of workout programs. Use pageNumber and pageSize query parameters.")
            .Produces<PagedResult<WorkoutProgramDto>>(StatusCodes.Status200OK);
        }
    }
}