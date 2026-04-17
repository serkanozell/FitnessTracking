using Nutrition.Application.Dtos;

namespace Nutrition.Application.Features.Foods.GetFoodById
{
    public sealed record GetFoodByIdQuery(Guid Id) : IQuery<Result<FoodDto>>;
}
