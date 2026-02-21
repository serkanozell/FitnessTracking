using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram
{
    internal sealed class GetWorkoutSessionsByProgramQueryHandler(IWorkoutSessionRepository _workoutSessionRepository) : IQueryHandler<GetWorkoutSessionsByProgramQuery, Result<IReadOnlyList<WorkoutSessionDto>>>
    {
        public async Task<Result<IReadOnlyList<WorkoutSessionDto>>> Handle(
            GetWorkoutSessionsByProgramQuery request,
            CancellationToken cancellationToken)
        {
            var sessions = await _workoutSessionRepository.GetListByProgramAsync(request.WorkoutProgramId, cancellationToken);

            return sessions.Select(WorkoutSessionDto.FromEntity).ToList();
        }
    }
}