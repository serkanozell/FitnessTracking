using MediatR;

public sealed class CreateWorkoutProgramCommand : IRequest<Guid>
{
    public string Name { get; init; } = default!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
