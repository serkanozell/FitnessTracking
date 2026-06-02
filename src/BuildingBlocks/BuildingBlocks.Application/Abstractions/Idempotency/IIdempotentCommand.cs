namespace BuildingBlocks.Application.Abstractions.Idempotency
{
    // Commands that must be protected against duplicate execution (e.g. a client
    // retrying a POST after a timeout) implement this marker. The key is supplied
    // by the caller through the "X-Idempotency-Key" HTTP header and propagated
    // into the command by the endpoint. When the key is null/empty the
    // IdempotencyBehavior is a no-op, so the command behaves normally.
    public interface IIdempotentCommand
    {
        string? IdempotencyKey { get; }
    }
}
