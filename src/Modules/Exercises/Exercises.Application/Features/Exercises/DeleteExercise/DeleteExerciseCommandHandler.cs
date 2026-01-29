namespace Exercises.Application.Features.Exercises.DeleteExercise
{
    public sealed class DeleteExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<DeleteExerciseCommand, bool>
    {
        public async Task<bool> Handle(
            DeleteExerciseCommand request,
            CancellationToken cancellationToken)
        {
            var exercise = await _exerciseRepository
                .GetByIdAsync(request.Id, cancellationToken);

            if (exercise is null)
            {
                return false;
            }

            exercise.Delete();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}