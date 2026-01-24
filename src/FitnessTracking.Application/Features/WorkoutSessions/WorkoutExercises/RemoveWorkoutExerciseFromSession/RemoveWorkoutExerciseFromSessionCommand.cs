using MediatR;

public sealed class RemoveWorkoutExerciseFromSessionCommand : IRequest<Unit>
{
    public Guid WorkoutSessionId { get; init; }
    public Guid WorkoutExerciseId { get; init; }
}