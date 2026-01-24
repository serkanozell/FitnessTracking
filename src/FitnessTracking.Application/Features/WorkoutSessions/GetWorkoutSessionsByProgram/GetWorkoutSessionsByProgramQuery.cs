using MediatR;

public sealed class GetWorkoutSessionsByProgramQuery : IRequest<IReadOnlyList<WorkoutSessionDto>>
{
    public Guid WorkoutProgramId { get; init; }
}