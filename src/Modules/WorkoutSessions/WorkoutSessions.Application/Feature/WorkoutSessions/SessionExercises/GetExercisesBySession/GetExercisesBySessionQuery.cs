using WorkoutSessions.Application.Dtos;

namespace WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.GetExercisesBySession
{
    public sealed class GetExercisesBySessionQuery : IQuery<IReadOnlyList<SessionExerciseDto>>
    {
        public Guid WorkoutSessionId { get; init; }
    }
}