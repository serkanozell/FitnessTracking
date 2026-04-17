using BuildingBlocks.Application.Abstractions;

namespace Nutrition.Application.Features.MealPlans.UpdateMealPlan
{
    internal sealed class UpdateMealPlanCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateMealPlanCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateMealPlanCommand request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            mealPlan.Update(request.Name, request.Date, request.Note);

            _repository.Update(mealPlan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
