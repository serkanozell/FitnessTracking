using MediatR;

public sealed record CreateExerciseCommand(string Name, string MuscleGroup, string Description) : IRequest<Guid>;