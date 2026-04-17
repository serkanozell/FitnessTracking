namespace Nutrition.Application.Features.Foods.DeleteFood
{
    internal sealed class DeleteFoodCommandHandler(
        IFoodRepository _repository,
        INutritionUnitOfWork _unitOfWork) : ICommandHandler<DeleteFoodCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteFoodCommand request, CancellationToken cancellationToken)
        {
            var food = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (food is null)
                return FoodErrors.NotFound(request.Id);

            if (food.IsDeleted)
                return FoodErrors.AlreadyDeleted(request.Id);

            food.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
