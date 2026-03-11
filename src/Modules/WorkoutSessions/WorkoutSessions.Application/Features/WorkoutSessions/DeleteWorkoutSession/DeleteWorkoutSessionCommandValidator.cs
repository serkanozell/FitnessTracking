namespace WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession
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