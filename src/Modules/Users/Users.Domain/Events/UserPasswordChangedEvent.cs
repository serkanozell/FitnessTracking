using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record UserPasswordChangedEvent(Guid UserId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
