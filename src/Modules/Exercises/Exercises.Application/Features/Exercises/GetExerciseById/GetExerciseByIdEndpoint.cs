using BuildingBlocks.Web;
using Exercises.Application.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Application.Features.Exercises.GetExerciseById
{
    public sealed class GetExerciseByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/exercises/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetExerciseByIdQuery(id), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Exercise not found.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("GetExerciseById")
            .WithTags("Exercises")
            .WithSummary("Gets an exercise by ID")
            .WithDescription("Returns a single exercise by its unique identifier")
            .Produces<ExerciseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}