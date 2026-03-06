using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.RemoveExerciseFromSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class RemoveExerciseFromSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly RemoveExerciseFromSessionCommandHandler _sut;

    public RemoveExerciseFromSessionCommandHandlerTests()
    {
        _sut = new RemoveExerciseFromSessionCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldRemoveEntry_WhenSessionAndExerciseExist()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        var entry = session.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        var command = new RemoveExerciseFromSessionCommand(session.Id, entry.Id);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        session.SessionExercises.Should().BeEmpty();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var command = new RemoveExerciseFromSessionCommand(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotFoundError_WhenEntryNotExists()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        var command = new RemoveExerciseFromSessionCommand(session.Id, Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.SessionExerciseNotFound");
    }
}
