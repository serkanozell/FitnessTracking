using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed class DeleteWorkoutProgramEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/api/workout-programs/{programId:guid}", async (Guid programId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteWorkoutProgramCommand(programId), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Delete failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("DeleteWorkoutProgram")
            .WithTags("WorkoutPrograms")
            .WithSummary("Deletes a workout program")
            .WithDescription("Deletes the workout program with the specified ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}