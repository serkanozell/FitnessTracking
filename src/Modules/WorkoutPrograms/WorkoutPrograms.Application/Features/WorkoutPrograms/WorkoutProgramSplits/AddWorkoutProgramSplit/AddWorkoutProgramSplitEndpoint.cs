using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    public sealed class AddWorkoutProgramSplitEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/workout-programs/{programId:guid}/splits", async (
                Guid programId,
                AddSplitRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new AddWorkoutProgramSplitCommand(programId, request.Name, request.Order);
                var result = await sender.Send(command, ct);

                return result.IsSuccess ? Results.Created($"/api/v1/workout-programs/{programId}/splits/{result.Data}", new AddSplitResponse(result.Data))
                                        : result.Error!.ToProblem("Add split failed.");
            })
            .WithName("AddWorkoutProgramSplit")
            .WithTags("WorkoutProgramSplits")
            .WithSummary("Adds a new split to a workout program")
            .Accepts<AddSplitRequest>("application/json")
            .Produces<AddSplitResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }

        public sealed record AddSplitRequest(string Name, int Order);
        public sealed record AddSplitResponse(Guid SplitId);
    }
}