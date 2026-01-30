using WorkoutPrograms.Application.Dtos;

namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits
{
    public sealed class GetWorkoutProgramSplitsQuery : IQuery<IReadOnlyList<WorkoutProgramSplitDto>>
    {
        public Guid WorkoutProgramId { get; init; }
    }
}