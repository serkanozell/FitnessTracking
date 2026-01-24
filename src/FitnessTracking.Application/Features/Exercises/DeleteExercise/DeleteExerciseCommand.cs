using MediatR;

public sealed record DeleteExerciseCommand(Guid Id) : IRequest<bool>;