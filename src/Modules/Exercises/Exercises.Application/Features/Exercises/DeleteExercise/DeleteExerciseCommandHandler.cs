using Exercises.Application.Errors;

namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    internal sealed class DeleteExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<DeleteExerciseCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(DeleteExerciseCommand request, CancellationToken cancellationToken)
        {
            var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (exercise is null)
                return ExerciseErrors.NotFound(request.Id);

            if (exercise.IsDeleted)
                return ExerciseErrors.AlreadyDeleted(request.Id);

            exercise.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}