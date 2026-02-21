using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById
{
    public sealed class GetWorkoutProgramByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/api/workoutprograms/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetWorkoutProgramByIdQuery(id), ct);

                return result.IsSuccess
                    ? Results.Ok(result.Data)
                    : Results.Problem(title: "Workout program not found.", detail: result.Error?.Message, statusCode: StatusCodes.Status404NotFound);
            })
            .WithName("GetWorkoutProgramById")
            .WithTags("WorkoutPrograms")
            .WithSummary("Gets a workout program by ID")
            .WithDescription("Returns a single workout program by its unique identifier")
            .Produces<WorkoutProgramDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}