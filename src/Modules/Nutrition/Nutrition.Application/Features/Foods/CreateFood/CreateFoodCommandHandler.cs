using BuildingBlocks.Application.Abstractions;
using Nutrition.Domain.Entity;
using Nutrition.Domain.Enums;

namespace Nutrition.Application.Features.Foods.CreateFood
{
    internal sealed class CreateFoodCommandHandler(
        IFoodRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<CreateFoodCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateFoodCommand request, CancellationToken cancellationToken)
        {
            var category = Enum.Parse<FoodCategory>(request.Category, ignoreCase: true);
            var servingUnit = Enum.Parse<ServingUnit>(request.ServingUnit, ignoreCase: true);
            var userId = Guid.Parse(_currentUser.UserId!);

            var food = Food.Create(
                request.Name,
                category,
                request.DefaultServingSize,
                servingUnit,
                request.Calories,
                request.Protein,
                request.Carbohydrates,
                request.Fat,
                request.Fiber,
                userId);

            await _repository.AddAsync(food, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return food.Id;
        }
    }
}
