namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise
{
    public sealed class RemoveSplitExerciseCommandValidator : AbstractValidator<RemoveSplitExerciseCommand>
    {
        public RemoveSplitExerciseCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId)
                .NotEmpty();

            RuleFor(x => x.WorkoutProgramSplitId)
                .NotEmpty();

            RuleFor(x => x.WorkoutProgramExerciseId)
                .NotEmpty();
        }
    }
}