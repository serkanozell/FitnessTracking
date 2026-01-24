using MediatR;

public sealed class AddExerciseToWorkoutProgramCommand : IRequest<Guid>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid ExerciseId { get; init; }
    public int Sets { get; init; }
    public int TargetReps { get; init; }
}