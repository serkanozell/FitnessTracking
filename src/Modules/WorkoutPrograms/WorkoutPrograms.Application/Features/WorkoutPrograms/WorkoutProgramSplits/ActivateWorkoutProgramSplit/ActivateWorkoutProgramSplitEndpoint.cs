using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

public sealed class ActivateWorkoutProgramSplitEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/workout-programs/{programId:guid}/splits/{splitId:guid}/activate", async (Guid programId,
                                                                                                        Guid splitId,
                                                                                                        ISender sender,
                                                                                                        CancellationToken ct) =>
        {
            var result = await sender.Send(new ActivateWorkoutProgramSplitCommand(programId, splitId), ct);

            return result.IsSuccess
                ? Results.Ok(new ActivateSplitResponse(result.Data))
                : Results.Problem(title: "Activate split failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
        })
        .WithName("ActivateWorkoutProgramSplit")
        .WithTags("WorkoutProgramSplits")
        .WithSummary("Activates a split in a workout program")
        .Produces<ActivateSplitResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public sealed record ActivateSplitResponse(Guid SplitId);
}