using BuildingBlocks.Application.Abstractions;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.UpdateMealItemQuantity
{
    internal sealed class UpdateMealItemQuantityCommandHandler(
        IMealPlanRepository _mealPlanRepository,
        IFoodRepository _foodRepository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateMealItemQuantityCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateMealItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(request.MealPlanId, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.MealPlanId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (mealPlan.Meals.All(m => m.Id != request.MealId))
                return MealPlanErrors.MealNotFound(request.MealPlanId, request.MealId);

            var meal = mealPlan.Meals.Single(m => m.Id == request.MealId);
            var existingItem = meal.MealItems.SingleOrDefault(i => i.Id == request.MealItemId);

            if (existingItem is null)
                return MealPlanErrors.MealItemNotFound(request.MealId, request.MealItemId);

            var food = await _foodRepository.GetByIdAsync(existingItem.FoodId, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(existingItem.FoodId);

            var macros = MacroNutrients.Calculate(food.Macros, request.Quantity, food.DefaultServingSize);

            mealPlan.UpdateMealItemQuantity(request.MealId, request.MealItemId, request.Quantity, macros);

            _mealPlanRepository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
