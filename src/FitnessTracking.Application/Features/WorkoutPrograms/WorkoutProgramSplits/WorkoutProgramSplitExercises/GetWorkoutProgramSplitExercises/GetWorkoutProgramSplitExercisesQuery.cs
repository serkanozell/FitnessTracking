using MediatR;

public sealed class GetWorkoutProgramSplitExercisesQuery : IRequest<IReadOnlyList<WorkoutProgramSplitExerciseDto>>
{
    public Guid WorkoutProgramId { get; init; }
    public Guid WorkoutSplitId { get; set; }
}
