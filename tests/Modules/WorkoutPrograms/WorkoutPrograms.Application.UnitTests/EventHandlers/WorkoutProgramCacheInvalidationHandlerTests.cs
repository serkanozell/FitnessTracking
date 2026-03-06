using BuildingBlocks.Application.Abstractions.Caching;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.EventHandlers;
using WorkoutPrograms.Domain.Events;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.EventHandlers;

public class WorkoutProgramCacheInvalidationHandlerTests
{
    private readonly ICacheService _cacheService = Substitute.For<ICacheService>();
    private readonly WorkoutProgramCacheInvalidationHandler _sut;

    public WorkoutProgramCacheInvalidationHandlerTests()
    {
        _sut = new WorkoutProgramCacheInvalidationHandler(_cacheService);
    }

    [Fact]
    public async Task Handle_WorkoutProgramCreatedEvent_ShouldInvalidateProgramAndListCache()
    {
        var programId = Guid.NewGuid();

        await _sut.Handle(new WorkoutProgramCreatedEvent(programId), CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutprograms:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutProgramUpdatedEvent_ShouldInvalidateProgramAndListCache()
    {
        var programId = Guid.NewGuid();

        await _sut.Handle(new WorkoutProgramUpdatedEvent(programId), CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutprograms:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutProgramActivatedEvent_ShouldInvalidateProgramAndListCache()
    {
        var programId = Guid.NewGuid();

        await _sut.Handle(new WorkoutProgramActivatedEvent(programId), CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutprograms:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutProgramDeletedEvent_ShouldInvalidateProgramAndListCache()
    {
        var programId = Guid.NewGuid();

        await _sut.Handle(new WorkoutProgramDeletedEvent(programId), CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveByPrefixAsync("workoutprograms:all", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WorkoutProgramSplitChangedEvent_ShouldInvalidateProgramAndSplitsCache()
    {
        var programId = Guid.NewGuid();

        await _sut.Handle(new WorkoutProgramSplitChangedEvent(programId), CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}:splits", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SplitExerciseChangedEvent_ShouldInvalidateProgramAndSplitExercisesCache()
    {
        var programId = Guid.NewGuid();
        var splitId = Guid.NewGuid();

        await _sut.Handle(new SplitExerciseChangedEvent(programId, splitId), CancellationToken.None);

        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}", Arg.Any<CancellationToken>());
        await _cacheService.Received(1).RemoveAsync($"workoutprograms:{programId}:splits:{splitId}:exercises", Arg.Any<CancellationToken>());
    }
}
