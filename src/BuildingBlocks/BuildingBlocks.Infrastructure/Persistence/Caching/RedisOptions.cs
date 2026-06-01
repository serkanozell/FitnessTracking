namespace BuildingBlocks.Infrastructure.Persistence.Caching
{
    public sealed class RedisOptions
    {
        public string ConnectionString { get; init; } = string.Empty;
        public int DefaultDatabase { get; init; } = 0;
    }
}