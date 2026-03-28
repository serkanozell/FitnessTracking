using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed class CreateExerciseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/exercises", async (CreateExerciseRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new CreateExerciseCommand(request.Name, request.PrimaryMuscleGroup, request.SecondaryMuscleGroup, request.Description, request.ImageUrl, request.VideoUrl),
                    ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/exercises/{result.Data}", new CreateExerciseResponse(result.Data))
                    : result.Error!.ToProblem("Create exercise failed.");
            })
                .WithName("CreateExercise")
                .WithTags("Exercises")
                .WithSummary("Creates a new exercise")
                .WithDescription("Creates a new exercise with name, muscle group and description")
                .Accepts<CreateExerciseRequest>("application/json")
                .Produces<CreateExerciseResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization("Admin");

        }

        public sealed record CreateExerciseRequest(string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description, string? ImageUrl, string? VideoUrl);

        public sealed record CreateExerciseResponse(Guid Id);
    }
}