using Users.Contracts.Events;
using Users.Domain.Events;

namespace Users.Application.Features.Users.EventHandlers
{
    internal sealed class UserDeletedEventHandler(IPublisher _publisher) : IDomainEventHandler<UserDeletedEvent>
    {
        public async Task Handle(UserDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _publisher.Publish(new UserDeletedIntegrationEvent(notification.UserId, notification.DeletedBy), cancellationToken);
        }
    }
}