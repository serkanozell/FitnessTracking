using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.UpdateWorkoutSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class UpdateWorkoutSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly UpdateWorkoutSessionCommandHandler _sut;

    public UpdateWorkoutSessionCommandHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new UpdateWorkoutSessionCommandHandler(_repository, _unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldUpdateDate_WhenSessionExists()
    {
        var session = WorkoutSession.Create(TestUserId, Guid.NewGuid(), new DateTime(2025, 6, 1));
        var newDate = new DateTime(2025, 7, 1);
        var command = new UpdateWorkoutSessionCommand(session.Id, newDate);
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        session.Date.Should().Be(newDate);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var command = new UpdateWorkoutSessionCommand(Guid.NewGuid(), DateTime.Now);
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }
}
