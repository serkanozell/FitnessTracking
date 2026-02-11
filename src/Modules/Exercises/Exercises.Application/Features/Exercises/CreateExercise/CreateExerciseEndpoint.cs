using BuildingBlocks.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed class CreateExerciseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/exercises", async (CreateExerciseRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new CreateExerciseCommand(request.Name, request.MuscleGroup, request.Description),
                    ct);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: "Create exercise failed.",
                                           detail: result.Error?.Message,
                                           statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.Created($"/api/exercises/{result.Data}", new CreateExerciseResponse(result.Data));
            })
                .WithName("CreateExercise")
                .WithTags("Exercises")
                .WithSummary("Creates a new exercise")
                .WithDescription("Creates a new exercise with name, muscle group and description")
                .Accepts<CreateExerciseRequest>("application/json")
                .Produces<CreateExerciseResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);
            // auth yapısı sonrası RequireAuthorization eklenecek

        }

        public sealed record CreateExerciseRequest(string Name, string MuscleGroup, string Description);

        public sealed record CreateExerciseResponse(Guid Id);
    }
}