using BuildingBlocks.Domain.Abstractions;

namespace Users.Contracts.Events;

public sealed record UserDeletedIntegrationEvent(Guid UserId) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.Now;
}