using Exercises.Application.Errors;
using Exercises.Domain.Entity;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    internal sealed class CreateExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<CreateExerciseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
        {
            var existingExercise = await _exerciseRepository.GetByNameAsync(request.Name, cancellationToken);

            if (existingExercise is not null)
                return ExerciseErrors.DuplicateName(request.Name);

            var exercise = Exercise.Create(request.Name,
                                           request.MuscleGroup,
                                           request.Description);

            await _exerciseRepository.AddAsync(exercise, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(exercise.Id);
        }
    }
}