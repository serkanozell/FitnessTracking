namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession
{
    public sealed class RemoveExerciseFromSessionCommandValidator : AbstractValidator<RemoveExerciseFromSessionCommand>
    {
        public RemoveExerciseFromSessionCommandValidator()
        {
            RuleFor(x => x.WorkoutSessionId)
                .NotEmpty();

            RuleFor(x => x.SessionExerciseId)
                .NotEmpty();
        }
    }
}