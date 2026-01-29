namespace BuildingBlocks.Application.Abstractions.Caching
{
    public interface ICacheableQuery
    {
        string CacheKey { get; }
        TimeSpan? Expiration { get; }
    }
}