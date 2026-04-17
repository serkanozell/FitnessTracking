using BuildingBlocks.Application.Abstractions;
using Users.Contracts.Events;

namespace Nutrition.Application.Features.Foods.EventHandlers
{
    internal sealed class UserDeletedFoodsHandler(
        IFoodRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : IDomainEventHandler<UserDeletedIntegrationEvent>
    {
        public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var foods = await _repository.GetActiveByUserIdAsync(notification.UserId, cancellationToken);

            var userFoods = foods.Where(f => f.UserId == notification.UserId).ToList();

            if (userFoods.Count == 0) return;

            _currentUser.SetSystemActor(notification.PerformedBy);
            try
            {
                foreach (var food in userFoods)
                {
                    food.Delete();
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
