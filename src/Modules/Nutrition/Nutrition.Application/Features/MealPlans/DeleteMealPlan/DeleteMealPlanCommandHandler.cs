using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.DeleteMealPlan
{
    internal sealed class DeleteMealPlanCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteMealPlanCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteMealPlanCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            mealPlan.Delete(_currentUser.UserId ?? "system");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
