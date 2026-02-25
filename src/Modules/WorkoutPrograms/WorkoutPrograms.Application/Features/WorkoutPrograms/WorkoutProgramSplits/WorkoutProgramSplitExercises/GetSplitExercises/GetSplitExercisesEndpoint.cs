using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises
{
    public sealed class GetSplitExercisesEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/workout-programs/{programId:guid}/splits/{splitId:guid}/exercises", async (
                Guid programId,
                Guid splitId,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(new GetSplitExercisesQuery(programId, splitId), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Failed to retrieve exercises.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("GetSplitExercises")
            .WithTags("WorkoutProgramSplitExercises")
            .WithSummary("Gets all exercises in a split")
            .Produces<IReadOnlyList<WorkoutProgramSplitExerciseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}