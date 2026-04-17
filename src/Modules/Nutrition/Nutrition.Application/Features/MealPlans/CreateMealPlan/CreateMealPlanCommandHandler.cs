using BuildingBlocks.Application.Abstractions;
using Nutrition.Domain.Entity;

namespace Nutrition.Application.Features.MealPlans.CreateMealPlan
{
    internal sealed class CreateMealPlanCommandHandler(
        IMealPlanRepository _repository,
        INutritionUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<CreateMealPlanCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateMealPlanCommand request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_currentUser.UserId!);

            var mealPlan = MealPlan.Create(userId, request.Name, request.Date, request.Note);

            await _repository.AddAsync(mealPlan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return mealPlan.Id;
        }
    }
}
