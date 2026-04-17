using BuildingBlocks.Application.Abstractions;
using Users.Contracts.Events;

namespace Nutrition.Application.Features.MealPlans.EventHandlers
{
    internal sealed class UserDeletedMealPlansHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : IDomainEventHandler<UserDeletedIntegrationEvent>
    {
        public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var mealPlans = await _repository.GetActiveByUserIdAsync(notification.UserId, cancellationToken);

            if (mealPlans.Count == 0) return;

            _currentUser.SetSystemActor(notification.PerformedBy);
            try
            {
                foreach (var mealPlan in mealPlans)
                {
                    mealPlan.Delete(notification.PerformedBy);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            finally
            {
                _currentUser.ClearSystemActor();
            }
        }
    }
}
