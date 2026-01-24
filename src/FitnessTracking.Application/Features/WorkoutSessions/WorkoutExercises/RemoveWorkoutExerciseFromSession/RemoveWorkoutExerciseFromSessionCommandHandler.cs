using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class RemoveWorkoutExerciseFromSessionCommandHandler
    : IRequestHandler<RemoveWorkoutExerciseFromSessionCommand, Unit>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public RemoveWorkoutExerciseFromSessionCommandHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Unit> Handle(
        RemoveWorkoutExerciseFromSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);
        if (session is null)
        {
            return Unit.Value;
        }

        session.RemoveEntry(request.WorkoutExerciseId);

        await _sessionRepository.UpdateAsync(session, cancellationToken);

        return Unit.Value;
    }
}