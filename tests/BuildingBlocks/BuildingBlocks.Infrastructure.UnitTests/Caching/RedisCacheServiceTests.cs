using BuildingBlocks.Infrastructure.Persistence.Caching;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NSubstitute;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Caching;

public class RedisCacheServiceTests
{
    private readonly IDistributedCache _distributedCache = Substitute.For<IDistributedCache>();
    private readonly IConnectionMultiplexer _connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
    private readonly RedisCacheService _sut;

    public RedisCacheServiceTests()
    {
        var options = Options.Create(new RedisOptions { DefaultExpiryMin = 10 });
        _sut = new RedisCacheService(_distributedCache, _connectionMultiplexer, options);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDeserializedValue_WhenKeyExists()
    {
        var json = "{\"Name\":\"Test\"}";
        _distributedCache.GetAsync("key", Arg.Any<CancellationToken>())
            .Returns(Encoding.UTF8.GetBytes(json));

        var result = await _sut.GetAsync<TestDto>("key");

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefault_WhenKeyNotExists()
    {
        _distributedCache.GetAsync("missing", Arg.Any<CancellationToken>())
            .Returns((byte[]?)null);

        var result = await _sut.GetAsync<TestDto>("missing");

        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ShouldCallDistributedCacheSet()
    {
        var dto = new TestDto { Name = "Test" };

        await _sut.SetAsync("key", dto);

        await _distributedCache.Received(1).SetAsync(
            "key",
            Arg.Any<byte[]>(),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_ShouldUseProvidedExpiry()
    {
        var expiry = TimeSpan.FromMinutes(30);

        await _sut.SetAsync("key", "value", expiry);

        await _distributedCache.Received(1).SetAsync(
            "key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiry),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_ShouldUseDefaultExpiry_WhenNoExpiryProvided()
    {
        await _sut.SetAsync("key", "value");

        await _distributedCache.Received(1).SetAsync(
            "key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == TimeSpan.FromMinutes(10)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_ShouldNotSetExpiry_WhenNoExpiryAndNoDefault()
    {
        var options = Options.Create(new RedisOptions { DefaultExpiryMin = null });
        var sut = new RedisCacheService(_distributedCache, _connectionMultiplexer, options);

        await sut.SetAsync("key", "value");

        await _distributedCache.Received(1).SetAsync(
            "key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallDistributedCacheRemove()
    {
        await _sut.RemoveAsync("key");

        await _distributedCache.Received(1).RemoveAsync("key", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_ShouldSerializeValueAsJson()
    {
        var dto = new TestDto { Name = "Hello" };

        await _sut.SetAsync("key", dto);

        await _distributedCache.Received(1).SetAsync(
            "key",
            Arg.Is<byte[]>(b => Encoding.UTF8.GetString(b).Contains("Hello")),
            Arg.Any<DistributedCacheEntryOptions>(),
            Arg.Any<CancellationToken>());
    }

    public class TestDto
    {
        public string Name { get; set; } = default!;
    }
}
