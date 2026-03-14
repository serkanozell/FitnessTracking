using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.UpdateExerciseInSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class UpdateExerciseInSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly UpdateExerciseInSessionCommandHandler _sut;

    public UpdateExerciseInSessionCommandHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new UpdateExerciseInSessionCommandHandler(_repository, _unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldUpdateEntry_WhenSessionAndExerciseExist()
    {
        var session = WorkoutSession.Create(TestUserId, Guid.NewGuid(), DateTime.Now);
        var entry = session.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        var command = new UpdateExerciseInSessionCommand(session.Id, entry.Id, 2, 90m, 8);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        entry.SetNumber.Should().Be(2);
        entry.Weight.Should().Be(90m);
        entry.Reps.Should().Be(8);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var command = new UpdateExerciseInSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 2, 90m, 8);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotFoundError_WhenEntryNotExists()
    {
        var session = WorkoutSession.Create(TestUserId, Guid.NewGuid(), DateTime.Now);
        var command = new UpdateExerciseInSessionCommand(session.Id, Guid.NewGuid(), 2, 90m, 8);
        _repository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.SessionExerciseNotFound");
    }
}
