using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    public sealed class CreateWorkoutProgramEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/workout-programs", async (CreateWorkoutProgramRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateWorkoutProgramCommand(request.Name, request.StartDate, request.EndDate);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/workout-programs/{result.Data}", new CreateWorkoutProgramResponse(result.Data))
                    : Results.Problem(title: "Create failed.", detail: result.Error?.Message, statusCode: StatusCodes.Status400BadRequest);
            })
            .WithName("CreateWorkoutProgram")
            .WithTags("WorkoutPrograms")
            .WithSummary("Creates a new workout program")
            .WithDescription("Creates a new workout program with name, start date and end date")
            .Accepts<CreateWorkoutProgramRequest>("application/json")
            .Produces<CreateWorkoutProgramResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record CreateWorkoutProgramRequest(string Name, DateTime StartDate, DateTime EndDate);
        public sealed record CreateWorkoutProgramResponse(Guid Id);
    }
}