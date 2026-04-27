using Nutrition.Domain.ValueObjects;

namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.UpdateLogEntryQuantity
{
    internal sealed class UpdateLogEntryQuantityCommandHandler(
        IDailyNutritionLogRepository _logRepository,
        IFoodRepository _foodRepository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<UpdateLogEntryQuantityCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateLogEntryQuantityCommand request, CancellationToken cancellationToken)
        {
            var log = await _logRepository.GetByIdAsync(request.DailyNutritionLogId, cancellationToken);

            if (log is null)
                return DailyNutritionLogErrors.NotFound(request.DailyNutritionLogId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, log.UserId);
            if (ownershipError is not null)
                return ownershipError;

            var entry = log.Entries.SingleOrDefault(e => e.Id == request.EntryId);
            if (entry is null)
                return DailyNutritionLogErrors.EntryNotFound(request.DailyNutritionLogId, request.EntryId);

            var food = await _foodRepository.GetByIdAsync(entry.FoodId, cancellationToken);
            if (food is null)
                return FoodErrors.NotFound(entry.FoodId);

            var macros = MacroNutrients.Calculate(food.Macros, request.Quantity, food.DefaultServingSize);

            log.UpdateEntryQuantity(request.EntryId, request.Quantity, macros);

            _logRepository.Update(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
