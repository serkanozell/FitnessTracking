using BuildingBlocks.Application.Abstractions.Caching;

internal sealed class CacheAsideService(ICacheService cache) : ICacheAsideService
{
    // Per-key locks prevent cache stampede (thundering herd): when a popular key
    // expires, only one caller rebuilds it while the others await the result.
    //
    // The locks are reference-counted and removed once the last waiter releases them,
    // so high-cardinality keys (e.g. paged/filtered query keys) cannot leak SemaphoreSlim
    // instances for the lifetime of this singleton service.
    private static readonly Dictionary<string, LockRef> _locks = new();
    private static readonly object _locksGate = new();

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

        var gate = AcquireLock(key);
        var acquired = false;
        try
        {
            await gate.Semaphore.WaitAsync(cancellationToken);
            acquired = true;

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
            if (acquired)
                gate.Semaphore.Release();

            ReleaseLock(key);
        }
    }

    private static LockRef AcquireLock(string key)
    {
        lock (_locksGate)
        {
            if (!_locks.TryGetValue(key, out var lockRef))
            {
                lockRef = new LockRef();
                _locks[key] = lockRef;
            }

            lockRef.Count++;
            return lockRef;
        }
    }

    private static void ReleaseLock(string key)
    {
        lock (_locksGate)
        {
            if (!_locks.TryGetValue(key, out var lockRef))
                return;

            lockRef.Count--;
            if (lockRef.Count == 0)
            {
                _locks.Remove(key);
                lockRef.Semaphore.Dispose();
            }
        }
    }

    private sealed class LockRef
    {
        public SemaphoreSlim Semaphore { get; } = new(1, 1);
        public int Count { get; set; }
    }
}