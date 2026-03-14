using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.ActivateSessionExercise;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class ActivateSessionExerciseCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly ActivateSessionExerciseCommandHandler _sut;

    public ActivateSessionExerciseCommandHandlerTests()
    {
        _sut = new ActivateSessionExerciseCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateEntry_WhenSessionActiveAndEntryExists()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now);
        session.Activate();
        var entry = session.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        var command = new ActivateSessionExerciseCommand(session.Id, entry.Id);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(entry.Id);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var command = new ActivateSessionExerciseCommand(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotActiveError_WhenSessionNotActive()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now);
        var entry = session.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        var command = new ActivateSessionExerciseCommand(session.Id, entry.Id);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotActive");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotFoundError_WhenEntryNotExists()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now);
        session.Activate();
        var command = new ActivateSessionExerciseCommand(session.Id, Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.SessionExerciseNotFound");
    }
}
