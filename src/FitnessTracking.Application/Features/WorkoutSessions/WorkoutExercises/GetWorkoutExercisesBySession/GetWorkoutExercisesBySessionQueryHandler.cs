using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetWorkoutExercisesBySessionQueryHandler
    : IRequestHandler<GetWorkoutExercisesBySessionQuery, IReadOnlyList<WorkoutExerciseDto>>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public GetWorkoutExercisesBySessionQueryHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IReadOnlyList<WorkoutExerciseDto>> Handle(
        GetWorkoutExercisesBySessionQuery request,
        CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.WorkoutSessionId, cancellationToken);
        if (session is null)
        {
            return Array.Empty<WorkoutExerciseDto>();
        }

        return session.WorkoutExercises
            .Select(WorkoutExerciseDto.FromEntity)
            .ToArray();
    }
}