namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit
{
    public sealed class AddExerciseToSplitCommandValidator : AbstractValidator<AddExerciseToSplitCommand>
    {
        public AddExerciseToSplitCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId)
                .NotEmpty();

            RuleFor(x => x.WorkoutProgramSplitId)
                .NotEmpty();

            RuleFor(x => x.ExerciseId)
                .NotEmpty();

            RuleFor(x => x.Sets)
                .GreaterThan(0);

            RuleFor(x => x.MinimumReps)
                .GreaterThan(0);

            RuleFor(x => x.MaximumReps)
                .GreaterThanOrEqualTo(x => x.MinimumReps);
        }
    }
}