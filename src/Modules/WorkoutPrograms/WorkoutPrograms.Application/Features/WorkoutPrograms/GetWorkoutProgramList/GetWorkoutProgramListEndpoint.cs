using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    public sealed class GetWorkoutProgramListEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/workoutprograms", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetWorkoutProgramListQuery(), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Failed to retrieve workout programs.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
            })
            .WithName("GetWorkoutProgramList")
            .WithTags("WorkoutPrograms")
            .WithSummary("Gets all workout programs")
            .WithDescription("Returns a list of all workout programs")
            .Produces<IReadOnlyList<WorkoutProgramDto>>(StatusCodes.Status200OK);
        }
    }
}