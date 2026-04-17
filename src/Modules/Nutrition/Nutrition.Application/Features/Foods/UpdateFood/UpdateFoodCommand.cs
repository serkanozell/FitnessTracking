namespace Nutrition.Application.Features.Foods.UpdateFood
{
    public sealed record UpdateFoodCommand(
        Guid Id,
        string Name,
        string Category,
        decimal DefaultServingSize,
        string ServingUnit,
        decimal Calories,
        decimal Protein,
        decimal Carbohydrates,
        decimal Fat,
        decimal? Fiber) : ICommand<Result<bool>>;
}
