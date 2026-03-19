namespace BuildingBlocks.Application.Abstractions.Caching
{
    public interface ICacheInvalidatingCommand
    {
        string[] CacheKeysToInvalidate { get; }
        string[] CachePrefixesToInvalidate { get; }
    }
}
