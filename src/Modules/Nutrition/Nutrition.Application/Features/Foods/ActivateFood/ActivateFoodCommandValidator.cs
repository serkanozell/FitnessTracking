namespace Nutrition.Application.Features.Foods.ActivateFood
{
    public sealed class ActivateFoodCommandValidator : AbstractValidator<ActivateFoodCommand>
    {
        public ActivateFoodCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
