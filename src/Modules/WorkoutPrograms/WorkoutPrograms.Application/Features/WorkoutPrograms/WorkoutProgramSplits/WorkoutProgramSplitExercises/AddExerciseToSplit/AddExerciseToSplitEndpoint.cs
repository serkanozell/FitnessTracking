using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    public sealed class AddExerciseToSplitEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/workout-programs/{programId:guid}/splits/{splitId:guid}/exercises", async (Guid programId,
                                                                                                              Guid splitId,
                                                                                                              AddExerciseRequest request,
                                                                                                              ISender sender,
                                                                                                              CancellationToken ct) =>
            {
                var exerciseToSplitCommand = new AddExerciseToSplitCommand(programId,
                                                                           splitId,
                                                                           request.ExerciseId,
                                                                           request.Sets,
                                                                           request.MinimumReps,
                                                                           request.MaximumReps);

                var result = await sender.Send(exerciseToSplitCommand, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/workout-programs/{programId}/splits/{splitId}/exercises/{result.Data}", new AddExerciseResponse(result.Data))
                    : Results.Problem(title: "Add exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
            })
            .WithName("AddExerciseToSplit")
            .WithTags("WorkoutProgramSplitExercises")
            .WithSummary("Adds an exercise to a split")
            .Accepts<AddExerciseRequest>("application/json")
            .Produces<AddExerciseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record AddExerciseRequest(Guid ExerciseId, int Sets, int MinimumReps, int MaximumReps);
        public sealed record AddExerciseResponse(Guid ExerciseId);
    }
}