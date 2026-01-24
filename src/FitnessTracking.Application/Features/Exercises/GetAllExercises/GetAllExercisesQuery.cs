using MediatR;

public sealed record GetAllExercisesQuery : IRequest<IReadOnlyList<ExerciseDto>>;