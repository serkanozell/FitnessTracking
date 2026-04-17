namespace Nutrition.Application.Features.Foods.ActivateFood
{
    public sealed record ActivateFoodCommand(Guid Id) : ICommand<Result<Guid>>;
}
