namespace BuildingBlocks.Application.Abstractions.Caching
{
    public sealed class CacheOptions
    {
        public int DefaultExpirationMinutes { get; set; } = 10;
    }
}