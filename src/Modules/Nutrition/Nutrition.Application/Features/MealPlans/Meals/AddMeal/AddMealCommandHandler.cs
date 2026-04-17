using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.Meals.AddMeal
{
    internal sealed class AddMealCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<AddMealCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddMealCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.MealPlanId, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.MealPlanId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            var meal = mealPlan.AddMeal(request.Name, request.Order);

            _repository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return meal.Id;
        }
    }
}
