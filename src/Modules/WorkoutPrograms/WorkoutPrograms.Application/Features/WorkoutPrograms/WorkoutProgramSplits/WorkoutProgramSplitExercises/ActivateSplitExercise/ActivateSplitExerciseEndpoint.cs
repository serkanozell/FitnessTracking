using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

public sealed class ActivateSplitExerciseEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/workoutprograms/{programId:guid}/splits/{splitId:guid}/exercises/{exerciseId:guid}/activate", async (Guid programId,
                                                                                                                                    Guid splitId,
                                                                                                                                    Guid exerciseId,
                                                                                                                                    ISender sender,
                                                                                                                                    CancellationToken ct) =>
        {
            var result = await sender.Send(new ActivateSplitExerciseCommand(programId, splitId, exerciseId), ct);

            return result.IsSuccess
                ? Results.Ok(new ActivateExerciseResponse(result.Data))
                : Results.Problem(title: "Activate exercise failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("ActivateSplitExercise")
        .WithTags("WorkoutProgramSplitExercises")
        .WithSummary("Activates an exercise in a split")
        .Produces<ActivateExerciseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record ActivateExerciseResponse(Guid ExerciseId);
}