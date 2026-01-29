using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Infrastructure.Persistence.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

internal sealed class RedisCacheService(IDistributedCache _distributedCache, IOptions<RedisOptions> _redisOptions) : ICacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(json))
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions();
        if (expiry.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiry;
        }
        else if (_redisOptions.Value.DefaultExpiryMin.HasValue && _redisOptions.Value.DefaultExpiryMin.Value > 0)
        {
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_redisOptions.Value.DefaultExpiryMin.Value);
        }

        await _distributedCache.SetStringAsync(key, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var buffer = await _distributedCache.GetAsync(key, cancellationToken);
        return buffer is not null && buffer.Length > 0;
    }
}