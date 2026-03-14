using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record RoleActivatedEvent(Guid RoleId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
