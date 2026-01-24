using FitnessTracking.Domain.Entity;
using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class CreateWorkoutSessionCommandHandler
        : IRequestHandler<CreateWorkoutSessionCommand, Guid>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public CreateWorkoutSessionCommandHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Guid> Handle(
        CreateWorkoutSessionCommand request,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        var session = new WorkoutSession(
            id,
            request.WorkoutProgramId,
            request.Date);

        await _sessionRepository.AddAsync(session, cancellationToken);

        return id;
    }
}