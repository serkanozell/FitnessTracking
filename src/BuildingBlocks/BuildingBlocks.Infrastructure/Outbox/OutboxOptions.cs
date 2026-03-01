namespace BuildingBlocks.Infrastructure.Outbox
{
    public sealed class OutboxOptions
    {
        public const string SectionName = "Outbox";
        public int IntervalInSeconds { get; set; } = 10;
        public int BatchSize { get; set; } = 20;
    }
}