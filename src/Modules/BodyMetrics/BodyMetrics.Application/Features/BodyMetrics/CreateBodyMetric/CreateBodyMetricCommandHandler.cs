using BuildingBlocks.Application.Abstractions;
using BodyMetrics.Domain.Entity;

namespace BodyMetrics.Application.Features.BodyMetrics.CreateBodyMetric
{
    internal sealed class CreateBodyMetricCommandHandler(IBodyMetricRepository _repository,
                                                         IBodyMetricsUnitOfWork _unitOfWork,
                                                         ICurrentUser _currentUser) : ICommandHandler<CreateBodyMetricCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateBodyMetricCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var metric = BodyMetric.Create(userId,
                                           request.Date,
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
                                           request.Note);

            await _repository.AddAsync(metric, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return metric.Id;
        }
    }
}