using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise
{
    public sealed class UpdateSplitExerciseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/api/workout-programs/{programId:guid}/splits/{splitId:guid}/exercises/{splitExerciseId:guid}", async (Guid programId,
                                                                                                                                Guid splitId,
                                                                                                                                Guid splitExerciseId,
                                                                                                                               UpdateExerciseRequest request,
                                                                                                                               ISender sender,
                                                                                                                               CancellationToken ct) =>
            {
                var command = new UpdateSplitExerciseCommand(programId,
                                                              splitId,
                                                              splitExerciseId,
                                                             request.Sets,
                                                             request.MinimumReps,
                                                             request.MaximumReps);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.NoContent()
                    : Results.Problem(title: "Update exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("UpdateSplitExercise")
            .WithTags("WorkoutProgramSplitExercises")
            .WithSummary("Updates an exercise in a split")
            .Accepts<UpdateExerciseRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record UpdateExerciseRequest(int Sets, int MinimumReps, int MaximumReps);
    }
}