using MediatR;

public sealed class CreateWorkoutSessionCommand : IRequest<Guid>
{
    public Guid WorkoutProgramId { get; init; }
    public DateTime Date { get; init; }
}