using MediatR;

public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOn { get; }
    public string EventType => GetType().AssemblyQualifiedName;
}