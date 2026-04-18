using WorkoutSessions.Domain.Entity;

namespace WorkoutSessions.Application.Dtos
{
    public sealed class WorkoutSessionListItemDto
    {
        public Guid Id { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public Guid WorkoutProgramSplitId { get; init; }
        public DateTime Date { get; init; }

        public static WorkoutSessionListItemDto FromEntity(WorkoutSession entity) =>
            new()
            {
                Id = entity.Id,
                WorkoutProgramId = entity.WorkoutProgramId,
                WorkoutProgramSplitId = entity.WorkoutProgramSplitId,
                Date = entity.Date
            };
    }
}