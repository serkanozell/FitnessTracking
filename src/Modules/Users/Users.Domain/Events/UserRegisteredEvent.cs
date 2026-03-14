using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record UserRegisteredEvent(Guid UserId, string Email) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
