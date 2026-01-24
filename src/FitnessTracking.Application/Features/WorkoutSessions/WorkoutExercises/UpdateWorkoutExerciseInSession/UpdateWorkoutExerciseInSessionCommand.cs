using MediatR;

public sealed class UpdateWorkoutExerciseInSessionCommand : IRequest<Unit>
{
    public Guid WorkoutSessionId { get; init; }
    public Guid WorkoutExerciseId { get; init; }
    public int SetNumber { get; init; }
    public decimal Weight { get; init; }
    public int Reps { get; init; }
}
