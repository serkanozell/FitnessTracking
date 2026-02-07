namespace WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession
{
    public sealed class ActivateWorkoutSessionCommandValidator : AbstractValidator<ActivateWorkoutSessionCommand>
    {
        public ActivateWorkoutSessionCommandValidator()
        {
            RuleFor(x => x.WorkoutSessionId)
                .NotEmpty();
        }
    }
}