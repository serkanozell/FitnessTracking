using Users.Contracts.Events;

namespace BodyMetrics.Application.Features.BodyMetrics.EventHandlers
{
    internal sealed class UserDeletedBodyMetricsHandler(IBodyMetricRepository _repository,
                                                        IBodyMetricsUnitOfWork _unitOfWork) : IDomainEventHandler<UserDeletedIntegrationEvent>
    {
        public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var metrics = await _repository.GetActiveByUserIdAsync(notification.UserId, cancellationToken);

            foreach (var metric in metrics)
            {
                metric.Delete();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}