using BuildingBlocks.Application.Results;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById
{
    public sealed record GetWorkoutProgramByIdQuery(Guid Id) : IQuery<Result<WorkoutProgramDto>>;
}