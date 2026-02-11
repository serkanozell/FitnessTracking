using Exercises.Application.Errors;
using Exercises.Domain.Entity;

namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    internal sealed class ActivateExerciseCommandHandler(IExerciseRepository exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<ActivateExerciseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(ActivateExerciseCommand request, CancellationToken cancellationToken)
        {
            Exercise? exercise = await exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (exercise is null)
                return ExerciseErrors.NotFound(request.Id);

            if (exercise.IsActive)
                return ExerciseErrors.AlreadyActive(request.Id);

            exercise.Activate();

            await exerciseRepository.UpdateAsync(exercise, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return exercise.Id;
        }
    }
}