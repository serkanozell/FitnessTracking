namespace WorkoutSessions.Application.Feature.WorkoutSessions.DeleteWorkoutSession
{
    public sealed class DeleteWorkoutSessionCommandValidator : AbstractValidator<DeleteWorkoutSessionCommand>
    {
        public DeleteWorkoutSessionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}