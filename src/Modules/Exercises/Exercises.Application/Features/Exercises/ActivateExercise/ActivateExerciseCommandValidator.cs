namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    public sealed class ActivateExerciseCommandValidator : AbstractValidator<ActivateExerciseCommand>
    {
        public ActivateExerciseCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}