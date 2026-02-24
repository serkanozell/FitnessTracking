using Exercises.Application.Errors;
using Exercises.Application.Features.Exercises.UpdateExercise;
using Exercises.Domain.Enums;

internal sealed class UpdateExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<UpdateExerciseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
            return ExerciseErrors.NotFound(request.Id);

        var primaryMuscleGroup = Enum.Parse<MuscleGroup>(request.PrimaryMuscleGroup, ignoreCase: true);
        var secondaryMuscleGroup = request.SecondaryMuscleGroup is not null
            ? Enum.Parse<MuscleGroup>(request.SecondaryMuscleGroup, ignoreCase: true)
            : (MuscleGroup?)null;

        exercise.Update(request.Name, primaryMuscleGroup, secondaryMuscleGroup, request.Description);

        await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}