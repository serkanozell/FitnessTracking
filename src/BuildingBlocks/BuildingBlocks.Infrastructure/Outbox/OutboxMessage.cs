namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public DateTime OccurredOnUtc { get; set; }
        public DateTime? ProcessedOnUtc { get; set; }
        public string? Error { get; set; }        
    }
}