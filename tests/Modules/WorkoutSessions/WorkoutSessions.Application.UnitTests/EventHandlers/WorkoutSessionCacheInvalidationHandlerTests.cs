using BuildingBlocks.Application.Abstractions.Caching;
using NSubstitute;
using WorkoutSessions.Application.Feature.WorkoutSessions.EventHandlers;
using WorkoutSessions.Domain.Events;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.EventHandlers;

public class WorkoutSessionCacheInvalidationHandlerTests
{
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly WorkoutSessionCacheInvalidationHandler _sut;

    public WorkoutSessionCacheInvalidationHandlerTests()
    {
        _sut = new WorkoutSessionCacheInvalidationHandler(_cacheService);
    }

    [Fact]
    public async Task Handle_WorkoutSessionCreatedEvent_ShouldInvalidateListAndProgramCache()
    {
        var programId = Guid.NewGuid();
        var @event = new WorkoutSessionCreatedEvent(Guid.NewGuid(), programId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveByPrefixAsync("workoutsessions:all", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync($"workoutsessions:program:{programId}", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutSessionUpdatedEvent_ShouldInvalidateSessionAndListAndProgramCache()
    {
        var sessionId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var @event = new WorkoutSessionUpdatedEvent(sessionId, programId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutsessions:{sessionId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutsessions:all", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync($"workoutsessions:program:{programId}", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutSessionDeletedEvent_ShouldInvalidateSessionAndListAndProgramCache()
    {
        var sessionId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var @event = new WorkoutSessionDeletedEvent(sessionId, programId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutsessions:{sessionId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutsessions:all", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync($"workoutsessions:program:{programId}", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutSessionActivatedEvent_ShouldInvalidateSessionAndListCache()
    {
        var sessionId = Guid.NewGuid();
        var @event = new WorkoutSessionActivatedEvent(sessionId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutsessions:{sessionId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutsessions:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SessionExerciseChangedEvent_ShouldInvalidateSessionAndExercisesCache()
    {
        var sessionId = Guid.NewGuid();
        var @event = new SessionExerciseChangedEvent(sessionId);

        await _sut.Handle(@event, CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutsessions:{sessionId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync($"workoutsessions:{sessionId}:exercises", Arg.Any<CancellationToken>());
    }
}
