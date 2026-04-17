using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.MealPlans.GetMealPlans
{
    internal sealed class GetMealPlansQueryHandler(
        IMealPlanRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetMealPlansQuery, Result<PagedResult<MealPlanDto>>>
    {
        public async Task<Result<PagedResult<MealPlanDto>>> Handle(GetMealPlansQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (items, totalCount) = await _repository.GetPagedByUserAsync(userId, request.PageNumber, request.PageSize, cancellationToken);

            var dtos = items
                .Select(MealPlanDto.FromEntity)
                .ToList();

            return PagedResult<MealPlanDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
