using BuildingBlocks.Infrastructure.Persistence.Caching;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Polly;
using Polly.Registry;
using StackExchange.Redis;
using System.Text;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Caching;

public class RedisCacheServiceTests
{
    private readonly IDistributedCache _distributedCache = Substitute.For<IDistributedCache>();
    private readonly IConnectionMultiplexer _connectionMultiplexer = Substitute.For<IConnectionMultiplexer>();
    private readonly RedisCacheService _sut;

    public RedisCacheServiceTests()
    {
        _sut = new RedisCacheService(_distributedCache, _connectionMultiplexer, CreatePipelineProvider(), NullLogger<RedisCacheService>.Instance);
    }

    private static ResiliencePipelineProvider<string> CreatePipelineProvider()
    {
        var provider = Substitute.For<ResiliencePipelineProvider<string>>();
        provider.GetPipeline(Arg.Any<string>()).Returns(ResiliencePipeline.Empty);
        return provider;
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
    public async Task GetAsync_ShouldReturnDefault_WhenRedisThrows()
    {
        _distributedCache.GetAsync("boom", Arg.Any<CancellationToken>())
            .Returns<byte[]?>(_ => throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "down"));

        var result = await _sut.GetAsync<TestDto>("boom");

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
    public async Task SetAsync_ShouldNotSetExpiry_WhenNoExpiryProvided()
    {
        await _sut.SetAsync("key", "value");

        await _distributedCache.Received(1).SetAsync(
            "key",
            Arg.Any<byte[]>(),
            Arg.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SetAsync_ShouldNotThrow_WhenRedisThrows()
    {
        _distributedCache
            .When(c => c.SetAsync("key", Arg.Any<byte[]>(), Arg.Any<DistributedCacheEntryOptions>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new RedisConnectionException(ConnectionFailureType.UnableToConnect, "down"));

        var act = async () => await _sut.SetAsync("key", "value");

        await act.Should().NotThrowAsync();
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
