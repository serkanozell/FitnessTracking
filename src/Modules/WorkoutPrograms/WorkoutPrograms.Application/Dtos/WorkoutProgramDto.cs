using WorkoutPrograms.Domain.Entity;

namespace WorkoutPrograms.Application.Dtos
{
    public sealed class WorkoutProgramDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        public static WorkoutProgramDto FromEntity(WorkoutProgram entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate
            };
    }
}