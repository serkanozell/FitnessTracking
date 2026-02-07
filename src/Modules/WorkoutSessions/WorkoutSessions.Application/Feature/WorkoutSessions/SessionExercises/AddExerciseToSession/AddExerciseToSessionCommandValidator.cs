namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession
{
    public sealed class AddExerciseToSessionCommandValidator : AbstractValidator<AddExerciseToSessionCommand>
    {
        public AddExerciseToSessionCommandValidator()
        {
            RuleFor(x => x.WorkoutSessionId)
                .NotEmpty();

            RuleFor(x => x.WorkoutSplitId)
                .NotEmpty();

            RuleFor(x => x.ExerciseId)
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