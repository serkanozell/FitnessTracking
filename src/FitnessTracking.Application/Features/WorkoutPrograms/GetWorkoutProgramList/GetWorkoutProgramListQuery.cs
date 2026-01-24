using MediatR;

public sealed class GetWorkoutProgramListQuery : IRequest<IReadOnlyList<WorkoutProgramDto>>
{
}