using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class UpdateExerciseCommandHandler
    : IRequestHandler<UpdateExerciseCommand, bool>
{
    private readonly IExerciseRepository _exerciseRepository;

    public UpdateExerciseCommandHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

    public async Task<bool> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var exercise = await _exerciseRepository
            .GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
        {
            return false;
        }

        exercise.Update(
            request.Name,
            request.MuscleGroup,
            request.Description);

        await _exerciseRepository.UpdateAsync(exercise, cancellationToken);

        return true;
    }
}