namespace WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.ActivateSessionExercise
{
    public sealed class ActivateSessionExerciseCommandValidator : AbstractValidator<ActivateSessionExerciseCommand>
    {
        public ActivateSessionExerciseCommandValidator()
        {
            RuleFor(x => x.WorkoutSessionId)
                .NotEmpty();

            RuleFor(x => x.SessionExerciseId)
                .NotEmpty();
        }
    }
}