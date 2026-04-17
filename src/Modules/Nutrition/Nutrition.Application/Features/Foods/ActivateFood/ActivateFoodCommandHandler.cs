namespace Nutrition.Application.Features.Foods.ActivateFood
{
    internal sealed class ActivateFoodCommandHandler(
        IFoodRepository _repository,
        INutritionUnitOfWork _unitOfWork) : ICommandHandler<ActivateFoodCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateFoodCommand request, CancellationToken cancellationToken)
        {
            var food = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(request.Id);

            if (food.IsActive)
                return FoodErrors.AlreadyActive(request.Id);

            food.Activate();

            _repository.Update(food);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return food.Id;
        }
    }
}
