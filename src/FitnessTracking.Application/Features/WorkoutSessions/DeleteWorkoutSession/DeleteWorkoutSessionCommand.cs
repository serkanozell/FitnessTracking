using MediatR;

public sealed class DeleteWorkoutSessionCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
}