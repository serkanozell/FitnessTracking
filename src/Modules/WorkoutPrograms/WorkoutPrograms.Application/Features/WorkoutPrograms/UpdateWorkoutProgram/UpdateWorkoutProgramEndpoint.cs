using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.UpdateWorkoutProgram
{
    public sealed class UpdateWorkoutProgramEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/api/workout-programs/{programId:guid}", async (Guid programId, UpdateWorkoutProgramRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new UpdateWorkoutProgramCommand(programId, request.Name, request.StartDate, request.EndDate);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Update failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("UpdateWorkoutProgram")
            .WithTags("WorkoutPrograms")
            .WithSummary("Updates an existing workout program")
            .WithDescription("Updates the workout program with the specified ID")
            .Accepts<UpdateWorkoutProgramRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record UpdateWorkoutProgramRequest(string Name, DateTime StartDate, DateTime EndDate);
    }
}