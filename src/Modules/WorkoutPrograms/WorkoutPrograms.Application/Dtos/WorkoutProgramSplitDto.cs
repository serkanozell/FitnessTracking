namespace WorkoutPrograms.Application.Dtos
{
    public sealed class WorkoutProgramSplitDto
    {
        public Guid Id { get; init; }
        public Guid WorkoutProgramId { get; init; }
        public string Name { get; init; } = default!;
        public int Order { get; init; }
        public bool IsActive { get; init; }
        public bool IsDeleted { get; init; }
        public DateTime? CreatedDate { get; init; }
        public string? CreatedBy { get; init; }
        public DateTime? UpdatedDate { get; init; }
        public string? UpdatedBy { get; init; }
    }
}