using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetWorkoutSessionByIdQueryHandler
    : IRequestHandler<GetWorkoutSessionByIdQuery, WorkoutSessionDetailDto?>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public GetWorkoutSessionByIdQueryHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<WorkoutSessionDetailDto?> Handle(
        GetWorkoutSessionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
        {
            return null;
        }

        return WorkoutSessionDetailDto.FromEntity(session);
    }
}