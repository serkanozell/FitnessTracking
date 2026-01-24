using FitnessTracking.Domain.Repositories;
using MediatR;

public sealed class GetWorkoutSessionsByProgramQueryHandler
    : IRequestHandler<GetWorkoutSessionsByProgramQuery, IReadOnlyList<WorkoutSessionDto>>
{
    private readonly IWorkoutSessionRepository _sessionRepository;

    public GetWorkoutSessionsByProgramQueryHandler(IWorkoutSessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<IReadOnlyList<WorkoutSessionDto>> Handle(
        GetWorkoutSessionsByProgramQuery request,
        CancellationToken cancellationToken)
    {
        var sessions = await _sessionRepository.GetListByProgramAsync(
            request.WorkoutProgramId,
            cancellationToken);

        return sessions
            .Select(WorkoutSessionDto.FromEntity)
            .ToArray();
    }
}