namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    public sealed record ActivateExerciseCommand(Guid Id) : ICommand<Result<Guid>>;
}