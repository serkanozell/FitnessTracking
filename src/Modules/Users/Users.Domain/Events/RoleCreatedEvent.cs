using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record RoleCreatedEvent(Guid RoleId, string Name) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
