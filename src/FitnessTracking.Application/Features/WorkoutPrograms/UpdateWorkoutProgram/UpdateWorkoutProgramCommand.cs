using MediatR;

public sealed class UpdateWorkoutProgramCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
