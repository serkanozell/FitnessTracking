namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;

public sealed class ActivateWorkoutProgramSplitCommandValidator : AbstractValidator<ActivateWorkoutProgramSplitCommand>
{
    public ActivateWorkoutProgramSplitCommandValidator()
    {
        RuleFor(x => x.WorkoutProgramId)
            .NotEmpty();

        RuleFor(x => x.SplitId)
            .NotEmpty();
    }
}