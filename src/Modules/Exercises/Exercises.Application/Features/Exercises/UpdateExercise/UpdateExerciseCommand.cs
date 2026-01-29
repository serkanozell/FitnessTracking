namespace Exercises.Application.Features.Exercises.UpdateExercise
{
    public sealed record UpdateExerciseCommand(Guid Id,
                                               string Name,
                                               string MuscleGroup,
                                               string Description) : ICommand<bool>;
}