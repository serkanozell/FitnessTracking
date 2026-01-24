using MediatR;

public sealed class GetWorkoutSessionByIdQuery : IRequest<WorkoutSessionDetailDto?>
{
    public Guid Id { get; init; }
}