using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.Foods.GetFoods
{
    internal sealed class GetFoodsQueryHandler(
        IFoodRepository _repository,
        ICurrentUser _currentUser) : IQueryHandler<GetFoodsQuery, Result<PagedResult<FoodDto>>>
    {
        public async Task<Result<PagedResult<FoodDto>>> Handle(GetFoodsQuery request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var (dtos, totalCount) = await _repository.GetPagedAsync(userId, request.PageNumber, request.PageSize, FoodDto.Projection, cancellationToken);

            return PagedResult<FoodDto>.Create(dtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
