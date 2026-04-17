using Nutrition.Domain.Enums;

namespace Nutrition.Application.Features.Foods.UpdateFood
{
    internal sealed class UpdateFoodCommandHandler(
        IFoodRepository _repository,
        INutritionUnitOfWork _unitOfWork) : ICommandHandler<UpdateFoodCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateFoodCommand request, CancellationToken cancellationToken)
        {
            var food = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(request.Id);

            var category = Enum.Parse<FoodCategory>(request.Category, ignoreCase: true);
            var servingUnit = Enum.Parse<ServingUnit>(request.ServingUnit, ignoreCase: true);

            food.Update(
                request.Name,
                category,
                request.DefaultServingSize,
                servingUnit,
                request.Calories,
                request.Protein,
                request.Carbohydrates,
                request.Fat,
                request.Fiber);

            _repository.Update(food);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
