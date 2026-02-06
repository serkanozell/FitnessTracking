namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;

public sealed class ActivateSplitExerciseCommandValidator : AbstractValidator<ActivateSplitExerciseCommand>
{
    public ActivateSplitExerciseCommandValidator()
    {
        RuleFor(x => x.WorkoutProgramId)
            .NotEmpty();

        RuleFor(x => x.SplitId)
            .NotEmpty();

        RuleFor(x => x.WorkoutSplitExerciseId)
            .NotEmpty();
    }
}