using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.DailyNutritionLogs.GetDailyLogById
{
    internal sealed class GetDailyLogByIdQueryHandler(
        IDailyNutritionLogRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetDailyLogByIdQuery, Result<DailyNutritionLogDto>>
    {
        public async Task<Result<DailyNutritionLogDto>> Handle(GetDailyLogByIdQuery request, CancellationToken cancellationToken)
        {
            var log = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (log is null)
                return DailyNutritionLogErrors.NotFound(request.Id);

            var ownershipError = OwnershipGuard.CheckOwnership(_currentUser, log.UserId);
            if (ownershipError is not null)
                return ownershipError;

            return DailyNutritionLogDto.FromEntity(log);
        }
    }
}
