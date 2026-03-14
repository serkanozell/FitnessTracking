using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record RoleUpdatedEvent(Guid RoleId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
