using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.GetWorkoutSessionsByProgram
{
    public sealed record GetWorkoutSessionsByProgramQuery(Guid WorkoutProgramId) : IQuery<Result<IReadOnlyList<WorkoutSessionDto>>>;
}