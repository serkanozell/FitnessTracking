using MediatR;

public sealed class AddWorkoutExerciseToSessionCommand : IRequest<Guid>
{
    public Guid WorkoutSessionId { get; init; }
    public Guid ExerciseId { get; init; }
    public int SetNumber { get; init; }
    public decimal Weight { get; init; }
    public int Reps { get; init; }
}
