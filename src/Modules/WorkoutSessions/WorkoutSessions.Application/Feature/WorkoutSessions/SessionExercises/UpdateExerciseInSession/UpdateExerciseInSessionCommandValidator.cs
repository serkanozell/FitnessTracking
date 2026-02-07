namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.UpdateExerciseInSession
{
    public sealed class UpdateExerciseInSessionCommandValidator : AbstractValidator<UpdateExerciseInSessionCommand>
    {
        public UpdateExerciseInSessionCommandValidator()
        {
            RuleFor(x => x.WorkoutSessionId)
                .NotEmpty();

            RuleFor(x => x.SessionExerciseId)
                .NotEmpty();

            RuleFor(x => x.SetNumber)
                .GreaterThan(0);

            RuleFor(x => x.Weight)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Reps)
                .GreaterThan(0);
        }
    }
}