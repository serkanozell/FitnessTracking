using Exercises.Application.Features.Exercises.UpdateExercise;

public sealed class UpdateExerciseCommandHandler(IExerciseRepository _exerciseRepository, IExercisesUnitOfWork _unitOfWork) : ICommandHandler<UpdateExerciseCommand, bool>
{
    public async Task<bool> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
        {
            return false;
        }

        exercise.Update(request.Name,
                        request.MuscleGroup,
                        request.Description);

        await _exerciseRepository.UpdateAsync(exercise, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}