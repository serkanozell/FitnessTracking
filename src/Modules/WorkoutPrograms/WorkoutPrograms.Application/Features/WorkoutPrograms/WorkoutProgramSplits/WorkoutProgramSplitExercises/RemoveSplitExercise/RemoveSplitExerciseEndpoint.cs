using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    public sealed class RemoveSplitExerciseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/api/workoutprograms/{programId:guid}/splits/{splitId:guid}/exercises/{exerciseId:guid}", async (Guid programId,
                                                                                                                                  Guid splitId,
                                                                                                                                  Guid exerciseId,
                                                                                                                                  ISender sender,
                                                                                                                                  CancellationToken ct) =>
            {
                var result = await sender.Send(new RemoveSplitExerciseCommand(programId, splitId, exerciseId), ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Remove exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("RemoveSplitExercise")
            .WithTags("WorkoutProgramSplitExercises")
            .WithSummary("Removes an exercise from a split")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}