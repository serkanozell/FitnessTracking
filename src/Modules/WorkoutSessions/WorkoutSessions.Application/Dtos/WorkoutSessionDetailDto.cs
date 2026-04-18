using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Application.Dtos
{
    public sealed class WorkoutSessionDetailDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public Guid WorkoutProgramSplitId { get; init; }
        public DateTime Date { get; init; }
        public IReadOnlyList<SessionExerciseDto> Exercises { get; init; } = Array.Empty<SessionExerciseDto>();

        public static WorkoutSessionDetailDto FromEntity(WorkoutSession entity) =>
            new()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                WorkoutProgramId = entity.WorkoutProgramId,
                WorkoutProgramSplitId = entity.WorkoutProgramSplitId,
                Date = entity.Date,
                Exercises = entity.SessionExercises
                    .Select(SessionExerciseDto.FromEntity)
                    .ToArray()
            };
    }
}