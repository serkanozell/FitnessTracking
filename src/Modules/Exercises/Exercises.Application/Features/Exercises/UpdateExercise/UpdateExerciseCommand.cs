namespace Exercises.Application.Features.Exercises.UpdateExercise
{
    public sealed record UpdateExerciseCommand(Guid Id,
                                               string Name,
                                               string PrimaryMuscleGroup,
                                               string? SecondaryMuscleGroup,
                                               string Description) : ICommand<Result<bool>>;
}