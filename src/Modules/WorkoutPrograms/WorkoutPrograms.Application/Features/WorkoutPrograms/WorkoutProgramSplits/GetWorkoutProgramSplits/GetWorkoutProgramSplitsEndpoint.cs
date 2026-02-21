using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    public sealed class GetWorkoutProgramSplitsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/workoutprograms/{programId:guid}/splits", async (Guid programId,
                                                                                    ISender sender,
                                                                                    CancellationToken ct) =>
            {
                var result = await sender.Send(new GetWorkoutProgramSplitsQuery(programId), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Failed to retrieve splits.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("GetWorkoutProgramSplits")
            .WithTags("WorkoutProgramSplits")
            .WithSummary("Gets all splits for a workout program")
            .Produces<IReadOnlyList<WorkoutProgramSplitDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}