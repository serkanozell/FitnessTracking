using FitnessTracking.Domain.Repositories;
using MediatR;

internal sealed class GetWorkoutSessionsQueryHandler
    : IRequestHandler<GetWorkoutSessionsQuery, IReadOnlyList<WorkoutSessionDto>>
{
    private readonly IWorkoutSessionRepository _workoutSessionRepository;

    public GetWorkoutSessionsQueryHandler(IWorkoutSessionRepository workoutSessionRepository)
    {
        _workoutSessionRepository = workoutSessionRepository;
    }

    public async Task<IReadOnlyList<WorkoutSessionDto>> Handle(
        GetWorkoutSessionsQuery request,
        CancellationToken cancellationToken)
    {
        var sessions = await _workoutSessionRepository
            .GetAllAsync(cancellationToken);

        if (sessions.Count == 0)
        {
            return Array.Empty<WorkoutSessionDto>();
        }

        var result = sessions
            .Select(s => new WorkoutSessionDto
            {
                Id = s.Id,
                WorkoutProgramId = s.WorkoutProgramId,
                Date = s.Date
            })
            .ToList()
            .AsReadOnly();

        return result;
    }
}