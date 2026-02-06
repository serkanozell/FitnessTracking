namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit
{
    public sealed class AddWorkoutProgramSplitCommandValidator : AbstractValidator<AddWorkoutProgramSplitCommand>
    {
        public AddWorkoutProgramSplitCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Order)
                .GreaterThan(0);
        }
    }
}