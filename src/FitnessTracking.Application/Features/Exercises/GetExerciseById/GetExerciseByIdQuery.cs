using MediatR;

public sealed record GetExerciseByIdQuery(Guid Id) : IRequest<ExerciseDto?>;