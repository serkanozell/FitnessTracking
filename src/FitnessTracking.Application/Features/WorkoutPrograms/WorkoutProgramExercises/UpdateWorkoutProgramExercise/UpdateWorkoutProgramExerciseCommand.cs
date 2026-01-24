using MediatR;

public sealed class UpdateWorkoutProgramExerciseCommand : IRequest<bool>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid WorkoutProgramExerciseId { get; init; }
    public int Sets { get; init; }
    public int TargetReps { get; init; }
}