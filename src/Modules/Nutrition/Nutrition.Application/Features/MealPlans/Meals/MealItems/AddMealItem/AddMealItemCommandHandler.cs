using BuildingBlocks.Application.Abstractions;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.AddMealItem
{
    internal sealed class AddMealItemCommandHandler(
        IMealPlanRepository _mealPlanRepository,
        IFoodRepository _foodRepository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<AddMealItemCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddMealItemCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(request.MealPlanId, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.MealPlanId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (mealPlan.Meals.All(m => m.Id != request.MealId))
                return MealPlanErrors.MealNotFound(request.MealPlanId, request.MealId);

            var food = await _foodRepository.GetByIdAsync(request.FoodId, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(request.FoodId);

            var macros = MacroNutrients.Calculate(food.Macros, request.Quantity, food.DefaultServingSize);

            var item = mealPlan.AddItemToMeal(
                request.MealId,
                food.Id,
                food.Name,
                request.Quantity,
                food.ServingUnit,
                macros);

            _mealPlanRepository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return item.Id;
        }
    }
}
