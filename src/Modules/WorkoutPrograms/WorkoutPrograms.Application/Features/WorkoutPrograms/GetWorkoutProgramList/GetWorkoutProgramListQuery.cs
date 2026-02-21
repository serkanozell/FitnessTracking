using BuildingBlocks.Application.Results;
using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    public sealed record GetWorkoutProgramListQuery : IQuery<Result<IReadOnlyList<WorkoutProgramDto>>>;
}