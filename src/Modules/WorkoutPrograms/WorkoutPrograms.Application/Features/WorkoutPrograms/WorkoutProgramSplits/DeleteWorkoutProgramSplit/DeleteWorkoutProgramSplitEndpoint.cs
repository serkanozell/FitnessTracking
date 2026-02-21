using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    public sealed class DeleteWorkoutProgramSplitEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/api/workoutprograms/{programId:guid}/splits/{splitId:guid}", async (Guid programId,
                                                                                                      Guid splitId,
                                                                                                      ISender sender,
                                                                                                      CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteWorkoutProgramSplitCommand(programId, splitId), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Delete split failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("DeleteWorkoutProgramSplit")
            .WithTags("WorkoutProgramSplits")
            .WithSummary("Deletes a split from a workout program")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}