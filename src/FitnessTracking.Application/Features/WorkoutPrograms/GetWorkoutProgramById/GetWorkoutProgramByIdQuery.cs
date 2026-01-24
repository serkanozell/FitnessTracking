using MediatR;

public sealed class GetWorkoutProgramByIdQuery : IRequest<WorkoutProgramDto?>
{
    public Guid Id { get; init; }
}