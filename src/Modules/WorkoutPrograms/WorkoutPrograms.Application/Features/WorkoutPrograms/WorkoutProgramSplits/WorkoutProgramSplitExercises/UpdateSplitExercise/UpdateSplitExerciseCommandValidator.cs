namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise
{
    public sealed class UpdateSplitExerciseCommandValidator : AbstractValidator<UpdateSplitExerciseCommand>
    {
        public UpdateSplitExerciseCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId)
                .NotEmpty();

            RuleFor(x => x.WorkoutProgramSplitId)
                .NotEmpty();

            RuleFor(x => x.WorkoutProgramExerciseId)
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