using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    public sealed record GetWorkoutProgramSplitsQuery(Guid WorkoutProgramId) : IQuery<Result<IReadOnlyList<WorkoutProgramSplitDto>>>;
}