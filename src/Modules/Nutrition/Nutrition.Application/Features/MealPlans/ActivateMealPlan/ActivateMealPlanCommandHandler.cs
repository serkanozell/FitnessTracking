using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.ActivateMealPlan
{
    internal sealed class ActivateMealPlanCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<ActivateMealPlanCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateMealPlanCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (mealPlan.IsActive)
                return MealPlanErrors.AlreadyActive(request.Id);

            mealPlan.Activate();

            _repository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return mealPlan.Id;
        }
    }
}
