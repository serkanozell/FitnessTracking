namespace Nutrition.Application.Features.DailyNutritionLogs.DeleteDailyLog
{
    internal sealed class DeleteDailyLogCommandHandler(
        IDailyNutritionLogRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteDailyLogCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteDailyLogCommand request, CancellationToken cancellationToken)
        {
            var log = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (log is null)
                return DailyNutritionLogErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, log.UserId);
            if (ownershipError is not null)
                return ownershipError;

            log.Delete(_currentUser.UserId ?? "system");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
