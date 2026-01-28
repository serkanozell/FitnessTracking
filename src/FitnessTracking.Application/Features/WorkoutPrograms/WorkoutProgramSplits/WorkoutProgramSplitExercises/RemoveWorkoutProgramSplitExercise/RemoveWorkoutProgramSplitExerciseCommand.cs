using MediatR;

public sealed class RemoveWorkoutProgramSplitExerciseCommand : IRequest<bool>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid WorkoutProgramSplitId { get; init; }
    public Guid WorkoutProgramExerciseId { get; init; }
}