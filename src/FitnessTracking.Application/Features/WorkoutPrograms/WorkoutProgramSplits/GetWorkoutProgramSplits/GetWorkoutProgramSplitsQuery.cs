using FitnessTracking.Application.Dtos;
using MediatR;

public sealed class GetWorkoutProgramSplitsQuery : IRequest<IReadOnlyList<WorkoutProgramSplitDto>>
{
    public Guid WorkoutProgramId { get; init; }
}
