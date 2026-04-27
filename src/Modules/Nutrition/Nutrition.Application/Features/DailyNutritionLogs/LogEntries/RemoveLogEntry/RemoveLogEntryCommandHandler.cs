namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.RemoveLogEntry
{
    internal sealed class RemoveLogEntryCommandHandler(
        IDailyNutritionLogRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<RemoveLogEntryCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RemoveLogEntryCommand request, CancellationToken cancellationToken)
        {
            var log = await _repository.GetByIdAsync(request.DailyNutritionLogId, cancellationToken);

            if (log is null)
                return DailyNutritionLogErrors.NotFound(request.DailyNutritionLogId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, log.UserId);
            if (ownershipError is not null)
                return ownershipError;

            log.RemoveEntry(request.EntryId);

            _repository.Update(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
