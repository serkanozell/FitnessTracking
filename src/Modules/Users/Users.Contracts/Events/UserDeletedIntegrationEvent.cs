using BuildingBlocks.Domain.Abstractions;

namespace Users.Contracts.Events;

public sealed record UserDeletedIntegrationEvent(Guid UserId, string PerformedBy) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.Now;
}