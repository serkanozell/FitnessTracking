using BuildingBlocks.Application.Abstractions.Caching;

internal sealed class CacheAsideService(ICacheService cache) : ICacheAsideService
{
    public async Task<T> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var value = await factory(cancellationToken);

        await cache.SetAsync(key, value, expiration, cancellationToken);

        return value;
    }
}