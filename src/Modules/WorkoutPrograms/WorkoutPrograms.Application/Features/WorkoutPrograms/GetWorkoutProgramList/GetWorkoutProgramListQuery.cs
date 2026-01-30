using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList
{
    public sealed class GetWorkoutProgramListQuery : IQuery<IReadOnlyList<WorkoutProgramDto>>
    {
    }
}