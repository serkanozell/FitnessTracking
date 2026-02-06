namespace WorkoutPrograms.Application.Features.WorkoutPrograms.DeleteWorkoutProgram
{
    public sealed class DeleteWorkoutProgramCommandValidator : AbstractValidator<DeleteWorkoutProgramCommand>
    {
        public DeleteWorkoutProgramCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}