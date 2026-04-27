namespace Nutrition.Application.Features.DailyNutritionLogs.UpdateDailyLog
{
    internal sealed class UpdateDailyLogCommandHandler(
        IDailyNutritionLogRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateDailyLogCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDailyLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (log is null)
                return DailyNutritionLogErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, log.UserId);
            if (ownershipError is not null)
                return ownershipError;

            log.Update(request.DailyCalorieGoal, request.Note);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
