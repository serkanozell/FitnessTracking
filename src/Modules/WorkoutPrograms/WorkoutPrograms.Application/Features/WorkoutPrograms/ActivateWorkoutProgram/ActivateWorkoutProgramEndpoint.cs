using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram
{
    public sealed class ActivateWorkoutProgramEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/api/workoutprograms/{id:guid}/activate", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ActivateWorkoutProgramCommand(id), ct);

                return result.IsSuccess
                    ? Results.Ok(new ActivateWorkoutProgramResponse(result.Data))
                    : Results.Problem(title: "Activation failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("ActivateWorkoutProgram")
            .WithTags("WorkoutPrograms")
            .WithSummary("Activates a workout program")
            .WithDescription("Activates a previously deactivated workout program")
            .Produces<ActivateWorkoutProgramResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record ActivateWorkoutProgramResponse(Guid Id);
    }
}