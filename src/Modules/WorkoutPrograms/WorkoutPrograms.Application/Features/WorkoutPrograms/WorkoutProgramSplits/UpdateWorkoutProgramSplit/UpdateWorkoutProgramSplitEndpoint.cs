using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    public sealed class UpdateWorkoutProgramSplitEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/api/workoutprograms/{programId:guid}/splits/{splitId:guid}", async (Guid programId,
                                                                                                   Guid splitId,
                                                                                                   UpdateSplitRequest request,
                                                                                                   ISender sender,
                                                                                                   CancellationToken ct) =>
            {
                var command = new UpdateWorkoutProgramSplitCommand(programId, splitId, request.Name, request.Order);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Update split failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("UpdateWorkoutProgramSplit")
            .WithTags("WorkoutProgramSplits")
            .WithSummary("Updates a split in a workout program")
            .Accepts<UpdateSplitRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record UpdateSplitRequest(string Name, int Order);
    }
}