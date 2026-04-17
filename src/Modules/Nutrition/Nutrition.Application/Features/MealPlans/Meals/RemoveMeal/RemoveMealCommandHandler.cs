using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.Meals.RemoveMeal
{
    internal sealed class RemoveMealCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<RemoveMealCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RemoveMealCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.MealPlanId, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.MealPlanId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (mealPlan.Meals.All(m => m.Id != request.MealId))
                return MealPlanErrors.MealNotFound(request.MealPlanId, request.MealId);

            mealPlan.RemoveMeal(request.MealId);

            _repository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
