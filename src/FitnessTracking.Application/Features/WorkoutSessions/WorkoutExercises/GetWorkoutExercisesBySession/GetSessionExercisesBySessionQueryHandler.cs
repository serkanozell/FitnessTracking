using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetSessionExercisesBySessionQueryHandler
    : IRequestHandler<GetSessionExercisesBySessionQuery, IReadOnlyList<SessionExerciseDto>>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public GetSessionExercisesBySessionQueryHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IReadOnlyList<SessionExerciseDto>> Handle(
        GetSessionExercisesBySessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);
        if (session is null)
        {
            return Array.Empty<SessionExerciseDto>();
        }

        return session.SessionExercises
            .Select(SessionExerciseDto.FromEntity)
            .ToArray();
    }
}