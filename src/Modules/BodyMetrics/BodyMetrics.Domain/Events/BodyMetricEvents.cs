using BuildingBlocks.Domain.Abstractions;

namespace BodyMetrics.Domain.Events
{
    public sealed record BodyMetricCreatedEvent(Guid BodyMetricId, Guid UserId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record BodyMetricUpdatedEvent(Guid BodyMetricId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record BodyMetricActivatedEvent(Guid BodyMetricId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }

    public sealed record BodyMetricDeletedEvent(Guid BodyMetricId) : IDomainEvent
    {
        public DateTime OccurredOn => DateTime.Now;
    }
}