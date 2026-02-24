namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed record CreateExerciseCommand(string Name, string PrimaryMuscleGroup, string? SecondaryMuscleGroup, string Description) : ICommand<Result<Guid>>;
}