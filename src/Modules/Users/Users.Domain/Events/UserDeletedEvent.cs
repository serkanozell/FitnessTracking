using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record UserDeletedEvent(Guid UserId, string DeletedBy) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
