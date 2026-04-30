using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramDetailView;

public sealed class GetWorkoutProgramDetailViewEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/workout-programs/{programId:guid}/detail-view",
            async (Guid programId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetWorkoutProgramDetailViewQuery(programId), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : result.Error!.ToProblem("Workout program not found.");
            })
            .WithName("GetWorkoutProgramDetailView")
            .WithTags("WorkoutPrograms")
            .WithSummary("Gets the aggregated detail view for a workout program in a single call")
            .Produces<WorkoutProgramDetailViewDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
