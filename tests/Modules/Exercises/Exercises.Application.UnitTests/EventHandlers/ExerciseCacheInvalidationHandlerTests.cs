using BuildingBlocks.Application.Abstractions.Caching;
using Exercises.Application.Features.Exercises.EventHandlers;
using Exercises.Domain.Events;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.EventHandlers;

public class ExerciseCacheInvalidationHandlerTests
{
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly ExerciseCacheInvalidationHandler _sut;

    public ExerciseCacheInvalidationHandlerTests()
    {
        _sut = new ExerciseCacheInvalidationHandler(_cacheService);
    }

    [Fact]
    public async Task Handle_ExerciseCreatedEvent_ShouldInvalidateCache()
    {
        var exerciseId = Guid.NewGuid();
        var @event = new ExerciseCreatedEvent(exerciseId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"exercises:{exerciseId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("exercises:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExerciseUpdatedEvent_ShouldInvalidateCache()
    {
        var exerciseId = Guid.NewGuid();
        var @event = new ExerciseUpdatedEvent(exerciseId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"exercises:{exerciseId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("exercises:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExerciseDeletedEvent_ShouldInvalidateCache()
    {
        var exerciseId = Guid.NewGuid();
        var @event = new ExerciseDeletedEvent(exerciseId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"exercises:{exerciseId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("exercises:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExerciseActivatedEvent_ShouldInvalidateCache()
    {
        var exerciseId = Guid.NewGuid();
        var @event = new ExerciseActivatedEvent(exerciseId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"exercises:{exerciseId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("exercises:all", Arg.Any<CancellationToken>());
    }
}
