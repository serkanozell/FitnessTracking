using MediatR;

public sealed class GetWorkoutProgramExercisesQuery : IRequest<IReadOnlyList<WorkoutProgramExerciseDto>>
{
    public Guid WorkoutProgramId { get; init; }
}
