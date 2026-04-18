namespace WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession
{
    public sealed class CreateWorkoutSessionCommandValidator : AbstractValidator<CreateWorkoutSessionCommand>
    {
        public CreateWorkoutSessionCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId).NotNull()
                                            .NotEmpty();

            RuleFor(x => x.WorkoutProgramSplitId).NotNull()
                                                 .NotEmpty();

            RuleFor(x => x.Date)
                .NotEmpty();
        }
    }
}