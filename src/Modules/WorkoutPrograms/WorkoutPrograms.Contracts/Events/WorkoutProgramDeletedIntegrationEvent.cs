using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Contracts.Events;

public sealed record WorkoutProgramDeletedIntegrationEvent(Guid ProgramId) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.UtcNow;
}
