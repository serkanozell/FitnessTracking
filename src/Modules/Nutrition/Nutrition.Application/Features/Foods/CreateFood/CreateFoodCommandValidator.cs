using Nutrition.Domain.Enums;

namespace Nutrition.Application.Features.Foods.CreateFood
{
    public sealed class CreateFoodCommandValidator : AbstractValidator<CreateFoodCommand>
    {
        public CreateFoodCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Category)
                .NotEmpty()
                .Must(v => Enum.TryParse<FoodCategory>(v, ignoreCase: true, out _))
                .WithMessage("Invalid food category.");

            RuleFor(x => x.DefaultServingSize)
                .GreaterThan(0);

            RuleFor(x => x.ServingUnit)
                .NotEmpty()
                .Must(v => Enum.TryParse<ServingUnit>(v, ignoreCase: true, out _))
                .WithMessage("Invalid serving unit.");

            RuleFor(x => x.Calories)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Protein)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Carbohydrates)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Fat)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Fiber)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Fiber.HasValue);
        }
    }
}
