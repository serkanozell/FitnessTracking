namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit
{
    public sealed class UpdateWorkoutProgramSplitCommandValidator : AbstractValidator<UpdateWorkoutProgramSplitCommand>
    {
        public UpdateWorkoutProgramSplitCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId)
                .NotEmpty();

            RuleFor(x => x.SplitId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Order)
                .GreaterThan(0);
        }
    }
}