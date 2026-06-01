using BuildingBlocks.Application.Abstractions.Caching;
using System.Collections.Concurrent;

internal sealed class CacheAsideService(ICacheService cache) : ICacheAsideService
{
    // Per-key locks to prevent cache stampede (thundering herd): when a popular
    // key expires, only one caller rebuilds it while others await the result.
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<T> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        Func<T, bool>? shouldCache = null,
        CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetAsync<T>(key, cancellationToken);
        if (cached is not null)
            return cached;

        var gate = _locks.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);
        try
        {
            // Double-check: another caller may have populated the cache while we waited.
            cached = await cache.GetAsync<T>(key, cancellationToken);
            if (cached is not null)
                return cached;

            var value = await factory(cancellationToken);

            if (shouldCache is null || shouldCache(value))
                await cache.SetAsync(key, value, expiration, cancellationToken);

            return value;
        }
        finally
        {
            gate.Release();
        }
    }
}