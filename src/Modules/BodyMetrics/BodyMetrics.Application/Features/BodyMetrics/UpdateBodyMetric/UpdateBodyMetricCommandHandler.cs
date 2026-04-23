using BuildingBlocks.Application.Abstractions;

namespace BodyMetrics.Application.Features.BodyMetrics.UpdateBodyMetric
{
    internal sealed class UpdateBodyMetricCommandHandler(
        IBodyMetricRepository _repository,
        IBodyMetricsUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateBodyMetricCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateBodyMetricCommand request, CancellationToken cancellationToken)
        {
            var metric = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (metric is null)
                return BodyMetricErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, metric.UserId);
            if (ownershipError is not null)
                return ownershipError;

            metric.Update(request.Date,
                          request.Weight,
                          request.Height,
                          request.BodyFatPercentage,
                          request.MuscleMass,
                          request.WaistCircumference,
                          request.ChestCircumference,
                          request.ArmCircumference,
                          request.HipCircumference,
                          request.ThighCircumference,
                          request.NeckCircumference,
                          request.ShoulderCircumference,
                          request.Note);

            _repository.Update(metric);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}