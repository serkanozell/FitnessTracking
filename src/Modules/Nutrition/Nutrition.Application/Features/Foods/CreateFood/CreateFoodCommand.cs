namespace Nutrition.Application.Features.Foods.CreateFood
{
    public sealed record CreateFoodCommand(
        string Name,
        string Category,
        decimal DefaultServingSize,
        string ServingUnit,
        decimal Calories,
        decimal Protein,
        decimal Carbohydrates,
        decimal Fat,
        decimal? Fiber) : ICommand<Result<Guid>>;
}
