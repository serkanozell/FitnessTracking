using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.CreateWorkoutProgram
{
    public sealed class CreateWorkoutProgramEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/workout-programs", async (CreateWorkoutProgramRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateWorkoutProgramCommand(request.Name, request.Description, request.StartDate, request.EndDate);
                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Created($"/api/v1/workout-programs/{result.Data}", new CreateWorkoutProgramResponse(result.Data))
                    : result.Error!.ToProblem("Create failed.");
            })
            .WithName("CreateWorkoutProgram")
            .WithTags("WorkoutPrograms")
            .WithSummary("Creates a new workout program")
            .WithDescription("Creates a new workout program with name, start date and end date")
            .Accepts<CreateWorkoutProgramRequest>("application/json")
            .Produces<CreateWorkoutProgramResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record CreateWorkoutProgramRequest(string Name, string? Description, DateTime StartDate, DateTime EndDate);
        public sealed record CreateWorkoutProgramResponse(Guid Id);
    }
}