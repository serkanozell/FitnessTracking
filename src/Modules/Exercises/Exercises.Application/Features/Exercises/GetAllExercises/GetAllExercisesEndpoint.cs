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
            endpoints.MapGet("/api/exercises", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAllExercisesQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Failed to retrieve exercises.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
            })
            .WithName("GetAllExercises")
            .WithTags("Exercises")
            .WithSummary("Gets all exercises")
            .WithDescription("Returns a list of all available exercises")
            .Produces<IReadOnlyList<ExerciseDto>>(StatusCodes.Status200OK);
        }
    }
}