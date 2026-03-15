using BuildingBlocks.Application.Abstractions;

namespace BodyMetrics.Application.Features.BodyMetrics.DeleteBodyMetric
{
    internal sealed class DeleteBodyMetricCommandHandler(
        IBodyMetricRepository _repository,
        IBodyMetricsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteBodyMetricCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteBodyMetricCommand request, CancellationToken cancellationToken)
        {
            var metric = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (metric is null)
                return BodyMetricErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, metric.UserId);
            if (ownershipError is not null)
                return ownershipError;

            metric.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}