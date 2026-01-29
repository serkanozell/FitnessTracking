using MediatR;

namespace BuildingBlocks.Domain.Abstractions
{
    public interface IDomainEvent : INotification
    {
        Guid EventId => Guid.NewGuid();
        DateTime OccurredOn { get; }
        public string EventType => GetType().AssemblyQualifiedName;
    }
}