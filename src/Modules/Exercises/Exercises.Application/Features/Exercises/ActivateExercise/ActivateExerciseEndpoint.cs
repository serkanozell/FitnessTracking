using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    public sealed class ActivateExerciseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/api/exercises/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ActivateExerciseCommand(id), ct);

                return result.IsSuccess
                    ? Results.Ok(new ActivateExerciseResponse(result.Data))
                    : Results.Problem(title: "Activation failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("ActivateExercise")
            .WithTags("Exercises")
            .WithSummary("Activates an exercise")
            .WithDescription("Activates a previously deactivated exercise")
            .Produces<ActivateExerciseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record ActivateExerciseResponse(Guid Id);
    }
}