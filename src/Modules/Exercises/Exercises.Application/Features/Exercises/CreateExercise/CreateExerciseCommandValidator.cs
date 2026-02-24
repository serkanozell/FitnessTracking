using Exercises.Domain.Enums;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    public sealed class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
    {
        public CreateExerciseCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .MaximumLength(100);

            RuleFor(x => x.PrimaryMuscleGroup)
                .NotEmpty()
                .Must(v => Enum.TryParse<MuscleGroup>(v, ignoreCase: true, out _))
                .WithMessage("Invalid primary muscle group.");

            RuleFor(x => x.SecondaryMuscleGroup)
                .Must(v => v is null || Enum.TryParse<MuscleGroup>(v, ignoreCase: true, out _))
                .WithMessage("Invalid secondary muscle group.");

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}