using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.Meals.MealItems.RemoveMealItem
{
    internal sealed class RemoveMealItemCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<RemoveMealItemCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RemoveMealItemCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.MealPlanId, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.MealPlanId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (mealPlan.Meals.All(m => m.Id != request.MealId))
                return MealPlanErrors.MealNotFound(request.MealPlanId, request.MealId);

            var meal = mealPlan.Meals.Single(m => m.Id == request.MealId);
            if (meal.MealItems.All(i => i.Id != request.MealItemId))
                return MealPlanErrors.MealItemNotFound(request.MealId, request.MealItemId);

            mealPlan.RemoveItemFromMeal(request.MealId, request.MealItemId);

            _repository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
