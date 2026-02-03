using Exercises.Domain.Entity;

namespace Exercises.Application.Features.Exercises.ActivateExercise
{
    internal sealed class ActivateExerciseCommandHandler(IExerciseRepository exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<ActivateExerciseCommand, Guid>
    {
        public async Task<Guid> Handle(ActivateExerciseCommand request, CancellationToken cancellationToken)
        {
            Exercise? exercise = await exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

            if (exercise is null)
            {
                throw new NotFoundException(nameof(Exercise), request.Id);
            }

            exercise.Activate();

            await exerciseRepository.UpdateAsync(exercise, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return exercise.Id;
        }
    }
}