using MediatR;

public sealed record UpdateExerciseCommand(
    Guid Id,
    string Name,
    string MuscleGroup,
    string Description
) : IRequest<bool>;