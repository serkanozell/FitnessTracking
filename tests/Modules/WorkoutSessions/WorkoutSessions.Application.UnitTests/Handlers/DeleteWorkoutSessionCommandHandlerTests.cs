using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.DeleteWorkoutSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class DeleteWorkoutSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly DeleteWorkoutSessionCommandHandler _sut;

    public DeleteWorkoutSessionCommandHandlerTests()
    {
        _sut = new DeleteWorkoutSessionCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDeleteSession_WhenExists()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.Now);
        var command = new DeleteWorkoutSessionCommand(session.Id);
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        session.IsDeleted.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new DeleteWorkoutSessionCommand(Guid.NewGuid());
        _repository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }
}
