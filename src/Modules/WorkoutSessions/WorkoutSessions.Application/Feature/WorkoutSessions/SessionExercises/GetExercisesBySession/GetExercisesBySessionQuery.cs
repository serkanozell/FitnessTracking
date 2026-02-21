using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.GetExercisesBySession
{
    public sealed record GetExercisesBySessionQuery(Guid WorkoutSessionId) : IQuery<Result<IReadOnlyList<SessionExerciseDto>>>;
}