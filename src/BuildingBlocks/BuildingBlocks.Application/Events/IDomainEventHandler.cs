using BuildingBlocks.Domain.Abstractions;
using MediatR;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent
{
}