namespace BuildingBlocks.Application.Abstractions.Idempotency
{
    public sealed class IdempotencyOptions
    {
        // How long a successful response is replayed for a repeated idempotency key.
        public int ExpirationMinutes { get; set; } = 60;
    }
}
