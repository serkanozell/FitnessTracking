using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Web;
using Exercises.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Application.Features.Exercises.GetAllExercises
{
    public sealed class GetAllExercisesEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/exercises", async (int? pageNumber, int? pageSize, ISender sender, CancellationToken ct) =>
            {
                var query = new GetAllExercisesQuery(pageNumber ?? PaginationDefaults.DefaultPageNumber,
                                                     pageSize ?? PaginationDefaults.DefaultPageSize);

                var result = await sender.Send(query, ct);

                return result.IsSuccess ? Results.Ok(result.Data)
                                        : Results.Problem(title: "Failed to retrieve exercises.",
                                                          detail: result.Error?.Message,
                                                          statusCode: StatusCodes.Status400BadRequest);
            })
            .WithName("GetAllExercises")
            .WithTags("Exercises")
            .WithSummary("Gets exercises with pagination")
            .WithDescription("Returns a paginated list of exercises. Use pageNumber and pageSize query parameters.")
            .Produces<PagedResult<ExerciseDto>>(StatusCodes.Status200OK);
        }
    }
}