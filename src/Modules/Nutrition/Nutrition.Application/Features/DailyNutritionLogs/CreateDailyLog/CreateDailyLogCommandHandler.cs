using Nutrition.Domain.Entity;

namespace Nutrition.Application.Features.DailyNutritionLogs.CreateDailyLog
{
    internal sealed class CreateDailyLogCommandHandler(
        IDailyNutritionLogRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<CreateDailyLogCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateDailyLogCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var existing = await _repository.GetByUserAndDateAsync(userId, request.Date, cancellationToken);
            if (existing is not null)
                return DailyNutritionLogErrors.AlreadyExistsForDate(request.Date);

            var log = DailyNutritionLog.Create(userId, request.Date, request.DailyCalorieGoal, request.Note);

            await _repository.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return log.Id;
        }
    }
}
