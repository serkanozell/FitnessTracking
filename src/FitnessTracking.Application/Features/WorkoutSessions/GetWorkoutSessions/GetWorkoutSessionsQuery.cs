using MediatR;

public sealed class GetWorkoutSessionsQuery
    : IRequest<IReadOnlyList<WorkoutSessionDto>>
{
}