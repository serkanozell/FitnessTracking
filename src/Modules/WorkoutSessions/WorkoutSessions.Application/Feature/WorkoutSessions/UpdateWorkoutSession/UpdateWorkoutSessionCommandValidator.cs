namespace WorkoutSessions.Application.Feature.WorkoutSessions.UpdateWorkoutSession
{
    public sealed class UpdateWorkoutSessionCommandValidator : AbstractValidator<UpdateWorkoutSessionCommand>
    {
        public UpdateWorkoutSessionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEmpty();

            RuleFor(x => x.Date)
                .NotEmpty();
        }
    }
}