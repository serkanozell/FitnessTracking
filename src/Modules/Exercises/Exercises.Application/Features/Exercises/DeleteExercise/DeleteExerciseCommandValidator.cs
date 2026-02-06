namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    public sealed class DeleteExerciseCommandValidator : AbstractValidator<DeleteExerciseCommand>
    {
        public DeleteExerciseCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}