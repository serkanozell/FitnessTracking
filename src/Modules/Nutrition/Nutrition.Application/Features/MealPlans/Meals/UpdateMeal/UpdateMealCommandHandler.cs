using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.Meals.UpdateMeal
{
    internal sealed class UpdateMealCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateMealCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.MealPlanId, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.MealPlanId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (mealPlan.Meals.All(m => m.Id != request.MealId))
                return MealPlanErrors.MealNotFound(request.MealPlanId, request.MealId);

            mealPlan.UpdateMeal(request.MealId, request.Name, request.Order);

            _repository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
