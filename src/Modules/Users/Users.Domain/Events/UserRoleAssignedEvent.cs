using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Events
{
    public sealed record UserRoleAssignedEvent(Guid UserId, Guid RoleId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}
