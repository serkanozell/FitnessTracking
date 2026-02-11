using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    public sealed class DeleteExerciseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/api/exercises/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteExerciseCommand(id), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Delete failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("DeleteExercise")
            .WithTags("Exercises")
            .WithSummary("Deletes an exercise")
            .WithDescription("Soft deletes the exercise with the specified ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}