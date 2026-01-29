using Exercises.Domain.Entity;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    internal sealed class CreateExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<CreateExerciseCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreateExerciseCommand request,
            CancellationToken cancellationToken)
        {
            var exercise = Exercise.Create(request.Name,
                                           request.MuscleGroup,
                                           request.Description);

            await _exerciseRepository.AddAsync(exercise, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return exercise.Id;
        }
    }
}