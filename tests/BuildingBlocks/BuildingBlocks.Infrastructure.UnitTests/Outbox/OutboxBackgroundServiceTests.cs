using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using BuildingBlocks.Infrastructure.Outbox;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Outbox;

public sealed record TestOutboxNotification(string Name) : INotification;

public class OutboxBackgroundServiceTests
{
    private static readonly string ResolvableType = typeof(TestOutboxNotification).AssemblyQualifiedName!;
    private static readonly string ValidContent = JsonSerializer.Serialize(new TestOutboxNotification("x"));

    private static (OutboxBackgroundService Sut, IMediator Mediator, Func<OutboxDbContext> DbFactory) CreateSut(OutboxOptions options)
    {
        var dbName = Guid.NewGuid().ToString();
        var root = new InMemoryDatabaseRoot();

        var mediator = Substitute.For<IMediator>();

        var services = new ServiceCollection();
        services.AddSingleton(mediator);
        services.AddDbContext<OutboxDbContext>(o => o.UseInMemoryDatabase(dbName, root));
        var provider = services.BuildServiceProvider();

        var sut = new OutboxBackgroundService(
            provider.GetRequiredService<IServiceScopeFactory>(),
            Options.Create(options),
            NullLogger<OutboxBackgroundService>.Instance);

        Func<OutboxDbContext> dbFactory = () => new OutboxDbContext(
            new DbContextOptionsBuilder<OutboxDbContext>().UseInMemoryDatabase(dbName, root).Options);

        return (sut, mediator, dbFactory);
    }

    private static OutboxMessage CreateMessage(string eventType, string content) => new()
    {
        Id = Guid.NewGuid(),
        EventType = eventType,
        Content = content,
        IsProcessed = false,
        OccurredOnUtc = DateTime.Now,
        RetryCount = 0
    };

    private static ConcurrentDictionary<string, Type> GetTypeCache()
    {
        var field = typeof(OutboxBackgroundService)
            .GetField("_eventTypeCache", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (ConcurrentDictionary<string, Type>)field.GetValue(null)!;
    }

    [Fact]
    public async Task ProcessOutboxMessagesAsync_ShouldCacheResolvedEventType_AndPublish()
    {
        var (sut, mediator, db) = CreateSut(new OutboxOptions { BatchSize = 10 });
        await using (var ctx = db())
        {
            ctx.OutboxMessages.Add(CreateMessage(ResolvableType, ValidContent));
            await ctx.SaveChangesAsync();
        }

        var processed = await sut.ProcessOutboxMessagesAsync(CancellationToken.None);

        processed.Should().Be(1);
        await mediator.Received(1).Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
        GetTypeCache().Should().ContainKey(ResolvableType);
    }

    [Fact]
    public async Task ProcessOutboxMessagesAsync_ShouldNotCacheUnresolvableType_AndMoveToDeadLetter()
    {
        const string unknownType = "Some.Unknown.Type, Some.Unknown.Assembly";
        var (sut, mediator, db) = CreateSut(new OutboxOptions { BatchSize = 10 });
        var id = Guid.NewGuid();
        await using (var ctx = db())
        {
            var msg = CreateMessage(unknownType, "{}");
            msg.Id = id;
            ctx.OutboxMessages.Add(msg);
            await ctx.SaveChangesAsync();
        }

        var processed = await sut.ProcessOutboxMessagesAsync(CancellationToken.None);

        processed.Should().Be(1);
        await mediator.DidNotReceive().Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
        GetTypeCache().Should().NotContainKey(unknownType);

        await using var assertCtx = db();
        var stored = await assertCtx.OutboxMessages.SingleAsync(m => m.Id == id);
        stored.IsProcessed.Should().BeTrue();
        stored.Error.Should().Contain("Could not resolve type");
    }

    [Fact]
    public async Task DrainOutboxAsync_ShouldDrainEntireBacklog_WhenBatchesAreFull()
    {
        // BatchSize 2, 5 messages => batches of 2, 2, 1 processed in a single drain cycle (no interval delay).
        var (sut, mediator, db) = CreateSut(new OutboxOptions { BatchSize = 2, MaxDrainIterations = 10 });
        await using (var ctx = db())
        {
            for (var i = 0; i < 5; i++)
                ctx.OutboxMessages.Add(CreateMessage(ResolvableType, ValidContent));
            await ctx.SaveChangesAsync();
        }

        await sut.DrainOutboxAsync(CancellationToken.None);

        await mediator.Received(5).Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await using var assertCtx = db();
        (await assertCtx.OutboxMessages.CountAsync(m => !m.IsProcessed)).Should().Be(0);
    }

    [Fact]
    public async Task DrainOutboxAsync_ShouldStopAtMaxDrainIterations()
    {
        // BatchSize 1 keeps every batch "full", so only MaxDrainIterations (3) batches run per cycle.
        var (sut, mediator, db) = CreateSut(new OutboxOptions { BatchSize = 1, MaxDrainIterations = 3 });
        await using (var ctx = db())
        {
            for (var i = 0; i < 10; i++)
                ctx.OutboxMessages.Add(CreateMessage(ResolvableType, ValidContent));
            await ctx.SaveChangesAsync();
        }

        await sut.DrainOutboxAsync(CancellationToken.None);

        await mediator.Received(3).Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await using var assertCtx = db();
        (await assertCtx.OutboxMessages.CountAsync(m => !m.IsProcessed)).Should().Be(7);
    }

    [Fact]
    public async Task DrainOutboxAsync_ShouldProcessOnce_WhenBatchIsNotFull()
    {
        // 3 messages with BatchSize 10 => one non-full batch, no extra drain iteration.
        var (sut, mediator, db) = CreateSut(new OutboxOptions { BatchSize = 10, MaxDrainIterations = 10 });
        await using (var ctx = db())
        {
            for (var i = 0; i < 3; i++)
                ctx.OutboxMessages.Add(CreateMessage(ResolvableType, ValidContent));
            await ctx.SaveChangesAsync();
        }

        await sut.DrainOutboxAsync(CancellationToken.None);

        await mediator.Received(3).Publish(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await using var assertCtx = db();
        (await assertCtx.OutboxMessages.CountAsync(m => !m.IsProcessed)).Should().Be(0);
    }
}
