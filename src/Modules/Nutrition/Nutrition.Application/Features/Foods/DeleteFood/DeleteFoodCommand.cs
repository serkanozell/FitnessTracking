namespace Nutrition.Application.Features.Foods.DeleteFood
{
    public sealed record DeleteFoodCommand(Guid Id) : ICommand<Result<bool>>;
}
