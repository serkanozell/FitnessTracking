using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class DeleteWorkoutSessionCommandHandler
    : IRequestHandler<DeleteWorkoutSessionCommand, Unit>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public DeleteWorkoutSessionCommandHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Unit> Handle(
        DeleteWorkoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            return Unit.Value; // idempotent
        }

        await _sessionRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}