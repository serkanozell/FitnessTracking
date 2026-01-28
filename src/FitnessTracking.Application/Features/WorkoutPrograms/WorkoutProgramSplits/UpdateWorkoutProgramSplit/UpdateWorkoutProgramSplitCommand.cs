using MediatR;

public sealed class UpdateWorkoutProgramSplitCommand : IRequest<bool>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid SplitId { get; init; }
    public string Name { get; init; } = default!;
    public int Order { get; init; }
}