using BuildingBlocks.Application.Abstractions.Caching;
using BuildingBlocks.Infrastructure.Resilience;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Registry;
using StackExchange.Redis;
using System.Text.Json;

internal sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ResiliencePipeline _pipeline;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(
        IDistributedCache distributedCache,
        IConnectionMultiplexer connectionMultiplexer,
        ResiliencePipelineProvider<string> pipelineProvider,
        ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
        _pipeline = pipelineProvider.GetPipeline(ResiliencePipelines.Redis);
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // Cache is a best-effort optimization: on any Redis failure we degrade to a
        // cache-miss so the caller falls back to the source of truth instead of failing.
        try
        {
            var json = await _pipeline.ExecuteAsync(
                async ct => await _distributedCache.GetStringAsync(key, ct),
                cancellationToken);

            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache GET failed for key {CacheKey}; treating as cache miss.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);

            var options = new DistributedCacheEntryOptions();
            if (expiry.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiry;
            }

            await _pipeline.ExecuteAsync(
                async ct => await _distributedCache.SetStringAsync(key, json, options, ct),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache SET failed for key {CacheKey}; value not cached.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _pipeline.ExecuteAsync(
                async ct => await _distributedCache.RemoveAsync(key, ct),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache REMOVE failed for key {CacheKey}.", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefixKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();

            foreach (var endpoint in _connectionMultiplexer.GetEndPoints())
            {
                var server = _connectionMultiplexer.GetServer(endpoint);

                await foreach (var key in server.KeysAsync(pattern: $"{prefixKey}*").WithCancellation(cancellationToken))
                {
                    await database.KeyDeleteAsync(key);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache REMOVE-BY-PREFIX failed for prefix {CachePrefix}.", prefixKey);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var buffer = await _pipeline.ExecuteAsync(
                async ct => await _distributedCache.GetAsync(key, ct),
                cancellationToken);

            return buffer is not null && buffer.Length > 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache EXISTS check failed for key {CacheKey}; treating as missing.", key);
            return false;
        }
    }
}