using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class DeleteExerciseCommandHandler
    : IRequestHandler<DeleteExerciseCommand, bool>
{
    private readonly IExerciseRepository _exerciseRepository;

    public DeleteExerciseCommandHandler(IExerciseRepository exerciseRepository)
    {
        _exerciseRepository = exerciseRepository;
    }

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

        await _exerciseRepository.DeleteAsync(exercise, cancellationToken);

        return true;
    }
}