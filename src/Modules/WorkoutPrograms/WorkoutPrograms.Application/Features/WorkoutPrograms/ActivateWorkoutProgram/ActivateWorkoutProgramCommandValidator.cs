namespace WorkoutPrograms.Application.Features.WorkoutPrograms.ActivateWorkoutProgram;

public sealed class ActivateWorkoutProgramCommandValidator : AbstractValidator<ActivateWorkoutProgramCommand>
{
    public ActivateWorkoutProgramCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}