namespace WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit
{
    public sealed class DeleteWorkoutProgramSplitCommandValidator : AbstractValidator<DeleteWorkoutProgramSplitCommand>
    {
        public DeleteWorkoutProgramSplitCommandValidator()
        {
            RuleFor(x => x.WorkoutProgramId)
                .NotEmpty();

            RuleFor(x => x.SplitId)
                .NotEmpty();
        }
    }
}