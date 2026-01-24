using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class UpdateWorkoutSessionCommandHandler
    : IRequestHandler<UpdateWorkoutSessionCommand, Unit>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public UpdateWorkoutSessionCommandHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Unit> Handle(
        UpdateWorkoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            throw new KeyNotFoundException($"WorkoutSession ({request.Id}) not found.");
        }

        session.UpdateDate(request.Date);

        await _sessionRepository.UpdateAsync(session, cancellationToken);

        return Unit.Value;
    }
}