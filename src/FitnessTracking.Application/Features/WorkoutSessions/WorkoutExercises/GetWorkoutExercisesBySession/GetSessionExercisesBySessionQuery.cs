using MediatR;

public sealed class GetSessionExercisesBySessionQuery : IRequest<IReadOnlyList<SessionExerciseDto>>
{
    public Guid WorkoutSessionId { get; init; }
}