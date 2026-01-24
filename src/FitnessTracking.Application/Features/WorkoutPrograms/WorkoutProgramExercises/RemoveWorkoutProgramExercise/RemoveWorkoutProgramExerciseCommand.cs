using MediatR;

public sealed class RemoveWorkoutProgramExerciseCommand : IRequest<bool>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid WorkoutProgramExerciseId { get; init; }
}