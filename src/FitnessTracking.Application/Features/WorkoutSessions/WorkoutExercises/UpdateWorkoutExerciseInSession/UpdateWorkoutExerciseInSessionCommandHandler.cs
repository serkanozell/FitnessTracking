using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class UpdateWorkoutExerciseInSessionCommandHandler
    : IRequestHandler<UpdateWorkoutExerciseInSessionCommand, Unit>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public UpdateWorkoutExerciseInSessionCommandHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Unit> Handle(
        UpdateWorkoutExerciseInSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);
        if (session is null)
        {
            throw new KeyNotFoundException($"WorkoutSession ({request.WorkoutSessionId}) not found.");
        }

        session.UpdateEntry(
            request.WorkoutExerciseId,
            request.SetNumber,
            request.Weight,
            request.Reps);

        await _sessionRepository.UpdateAsync(session, cancellationToken);

        return Unit.Value;
    }
}