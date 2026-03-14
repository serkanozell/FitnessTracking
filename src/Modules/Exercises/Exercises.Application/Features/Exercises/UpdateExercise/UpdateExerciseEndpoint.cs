using BuildingBlocks.Web;
using Exercises.Application.Features.Exercises.UpdateExercise;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public sealed class UpdateExerciseEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/exercises/{id:guid}", async (Guid id, UpdateExerciseRequest request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateExerciseCommand(id, request.Name, request.PrimaryMuscleGroup, request.SecondaryMuscleGroup, request.Description);
            var result = await sender.Send(command, ct);

            return result.IsSuccess
                ? Results.NoContent()
                : result.Error!.ToProblem("Update failed.");
        })
        .WithName("UpdateExercise")
        .WithTags("Exercises")
        .WithSummary("Updates an existing exercise")
        .WithDescription("Updates the exercise with the specified ID")
        .Accepts<UpdateExerciseRequest>("application/json")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .RequireAuthorization("Admin");
    }

    public sealed record UpdateExerciseRequest(string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description);
}