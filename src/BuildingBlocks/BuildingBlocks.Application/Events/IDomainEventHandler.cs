using BuildingBlocks.Domain.Abstractions;
using MediatR;

namespace BuildingBlocks.Application.Events
{
    public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent
    {
    }
}