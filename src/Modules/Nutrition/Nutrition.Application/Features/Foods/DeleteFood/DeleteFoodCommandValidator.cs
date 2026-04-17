namespace Nutrition.Application.Features.Foods.DeleteFood
{
    public sealed class DeleteFoodCommandValidator : AbstractValidator<DeleteFoodCommand>
    {
        public DeleteFoodCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
