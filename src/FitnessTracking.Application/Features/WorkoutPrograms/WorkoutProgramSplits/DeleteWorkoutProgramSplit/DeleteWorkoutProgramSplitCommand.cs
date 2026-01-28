using MediatR;

public sealed class DeleteWorkoutProgramSplitCommand : IRequest<bool>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid SplitId { get; init; }
}