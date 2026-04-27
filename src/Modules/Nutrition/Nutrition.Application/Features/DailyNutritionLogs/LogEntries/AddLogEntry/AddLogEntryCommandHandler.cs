using Nutrition.Domain.ValueObjects;

namespace Nutrition.Application.Features.DailyNutritionLogs.LogEntries.AddLogEntry
{
    internal sealed class AddLogEntryCommandHandler(
        IDailyNutritionLogRepository _logRepository,
        IFoodRepository _foodRepository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<AddLogEntryCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(AddLogEntryCommand request, CancellationToken cancellationToken)
        {
            var log = await _logRepository.GetByIdAsync(request.DailyNutritionLogId, cancellationToken);

            if (log is null)
                return DailyNutritionLogErrors.NotFound(request.DailyNutritionLogId);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, log.UserId);
            if (ownershipError is not null)
                return ownershipError;

            var food = await _foodRepository.GetByIdAsync(request.FoodId, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(request.FoodId);

            var macros = MacroNutrients.Calculate(food.Macros, request.Quantity, food.DefaultServingSize);

            var entry = log.AddEntry(food.Id, food.Name, request.Quantity, food.ServingUnit, macros);

            _logRepository.Update(log);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return entry.Id;
        }
    }
}
