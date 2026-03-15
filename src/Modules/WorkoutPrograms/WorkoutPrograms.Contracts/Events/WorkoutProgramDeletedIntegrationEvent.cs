using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Contracts.Events;

public sealed record WorkoutProgramDeletedIntegrationEvent(Guid ProgramId, string PerformedBy) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.Now;
}
