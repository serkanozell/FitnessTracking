using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.Foods.GetFoodById
{
    internal sealed class GetFoodByIdQueryHandler(
        IFoodRepository _repository) : IQueryHandler<GetFoodByIdQuery, Result<FoodDto>>
    {
        public async Task<Result<FoodDto>> Handle(GetFoodByIdQuery request, CancellationToken cancellationToken)
        {
            var food = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(request.Id);

            return FoodDto.FromEntity(food);
        }
    }
}
