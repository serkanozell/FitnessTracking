using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record UserDeletedEvent(Guid UserId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
