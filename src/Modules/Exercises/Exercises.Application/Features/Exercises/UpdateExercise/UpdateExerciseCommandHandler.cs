using Exercises.Application.Errors;
using Exercises.Application.Features.Exercises.UpdateExercise;

internal sealed class UpdateExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<UpdateExerciseCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
            return ExerciseErrors.NotFound(request.Id);

        exercise.Update(request.Name, request.MuscleGroup, request.Description);

        await _exerciseRepository.UpdateAsync(exercise, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}