using BuildingBlocks.Application.Abstractions;

namespace BodyMetrics.Application.Features.BodyMetrics.ActivateBodyMetric
{
    internal sealed class ActivateBodyMetricCommandHandler(
        IBodyMetricRepository _repository,
        IBodyMetricsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<ActivateBodyMetricCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateBodyMetricCommand request, CancellationToken cancellationToken)
        {
            var metric = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (metric is null)
                return BodyMetricErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, metric.UserId);
            if (ownershipError is not null)
                return ownershipError;

            if (metric.IsActive)
                return BodyMetricErrors.AlreadyActive(request.Id);

            metric.Activate();

            _repository.Update(metric);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return metric.Id;
        }
    }
}