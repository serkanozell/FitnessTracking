using BuildingBlocks.Application.Abstractions;
using Users.Contracts.Events;

namespace BodyMetrics.Application.Features.BodyMetrics.EventHandlers
{
    internal sealed class UserDeletedBodyMetricsHandler(
        IBodyMetricRepository _repository,
        IBodyMetricsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : IDomainEventHandler<UserDeletedIntegrationEvent>
    {
        public async Task Handle(UserDeletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            var metrics = await _repository.GetActiveByUserIdAsync(notification.UserId, cancellationToken);

            if (metrics.Count == 0) return;

            _currentUser.SetSystemActor(notification.PerformedBy);
            try
            {
                foreach (var metric in metrics)
                {
                    metric.Delete();
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