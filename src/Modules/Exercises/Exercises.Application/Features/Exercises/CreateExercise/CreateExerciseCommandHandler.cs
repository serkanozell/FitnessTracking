using Exercises.Application.Errors;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;

namespace Exercises.Application.Features.Exercises.CreateExercise
{
    internal sealed class CreateExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<CreateExerciseCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
        {
            var existingExercise = await _exerciseRepository.GetByNameAsync(request.Name, cancellationToken);

            if (existingExercise is not null)
                return ExerciseErrors.DuplicateName(request.Name);

            var primaryMuscleGroup = Enum.Parse<MuscleGroup>(request.PrimaryMuscleGroup, ignoreCase: true);
            var secondaryMuscleGroup = request.SecondaryMuscleGroup is not null
                ? Enum.Parse<MuscleGroup>(request.SecondaryMuscleGroup, ignoreCase: true)
                : (MuscleGroup?)null;

            var exercise = Exercise.Create(request.Name,
                                           primaryMuscleGroup,
                                           secondaryMuscleGroup,
                                           request.Description);

            await _exerciseRepository.AddAsync(exercise, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(exercise.Id);
        }
    }
}