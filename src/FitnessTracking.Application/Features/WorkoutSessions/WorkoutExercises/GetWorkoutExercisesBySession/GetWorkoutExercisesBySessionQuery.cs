using MediatR;

public sealed class GetWorkoutExercisesBySessionQuery : IRequest<IReadOnlyList<WorkoutExerciseDto>>
{
    public Guid WorkoutSessionId { get; init; }
}