using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Feature.WorkoutSessions.ActivateWorkoutSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class ActivateWorkoutSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly ActivateWorkoutSessionCommandHandler _sut;

    public ActivateWorkoutSessionCommandHandlerTests()
    {
        _sut = new ActivateWorkoutSessionCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateSession_WhenExistsAndNotActive()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        var command = new ActivateWorkoutSessionCommand(session.Id);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(session.Id);
        session.IsActive.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new ActivateWorkoutSessionCommand(Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyActiveError_WhenAlreadyActive()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        session.Activate();
        var command = new ActivateWorkoutSessionCommand(session.Id);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.AlreadyActive");
    }
}
