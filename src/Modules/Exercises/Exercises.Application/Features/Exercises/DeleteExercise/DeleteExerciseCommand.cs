namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    public sealed record DeleteExerciseCommand(Guid Id) : ICommand<bool>;
}