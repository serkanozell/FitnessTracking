using BuildingBlocks.Application.Pagination;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.DailyNutritionLogs.GetDailyLogs
{
    internal sealed class GetDailyLogsQueryHandler(
        IDailyNutritionLogRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetDailyLogsQuery, Result<PagedResult<DailyNutritionLogDto>>>
    {
        public async Task<Result<PagedResult<DailyNutritionLogDto>>> Handle(GetDailyLogsQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (items, totalCount) = await _repository.GetPagedByUserAsync(userId, request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items
                .Select(DailyNutritionLogDto.FromEntity)
                .ToList();

            return PagedResult<DailyNutritionLogDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
