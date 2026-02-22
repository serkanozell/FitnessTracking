namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed record CreateExerciseCommand(string Name, string MuscleGroup, string Description) : ICommand<Result<Guid>>;
}