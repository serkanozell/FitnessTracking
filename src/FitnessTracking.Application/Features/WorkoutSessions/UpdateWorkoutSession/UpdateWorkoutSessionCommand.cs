using MediatR;

public sealed class UpdateWorkoutSessionCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
}
