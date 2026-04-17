using Nutrition.Contracts.Events;
using Nutrition.Domain.Events;

namespace Nutrition.Application.Features.MealPlans.EventHandlers
{
    internal sealed class MealPlanDeletedEventHandler(IPublisher _publisher) : IDomainEventHandler<MealPlanDeletedEvent>
    {
        public async Task Handle(MealPlanDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new MealPlanDeletedIntegrationEvent(notification.MealPlanId, notification.DeletedBy), cancellationToken);
        }
    }
}
