using BuildingBlocks.Application.Abstractions;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.MealPlans.GetMealPlanById
{
    internal sealed class GetMealPlanByIdQueryHandler(
        IMealPlanRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetMealPlanByIdQuery, Result<MealPlanDto>>
    {
        public async Task<Result<MealPlanDto>> Handle(GetMealPlanByIdQuery request, CancellationToken cancellationToken)
        {
            var mealPlan = await _repository.GetByIdWithMealsAsync(request.Id, cancellationToken);

            if (mealPlan is null)
                return MealPlanErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, mealPlan.UserId);
            if (ownershipError is not null)
                return ownershipError;

            return MealPlanDto.FromEntity(mealPlan);
        }
    }
}
