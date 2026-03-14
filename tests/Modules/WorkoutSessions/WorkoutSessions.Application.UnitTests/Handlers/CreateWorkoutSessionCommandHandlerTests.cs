using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class CreateWorkoutSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly CreateWorkoutSessionCommandHandler _sut;

    public CreateWorkoutSessionCommandHandlerTests()
    {
        _currentUser.UserId.Returns(Guid.NewGuid().ToString());
        _sut = new CreateWorkoutSessionCommandHandler(_repository, _unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldCreateSession()
    {
        var command = new CreateWorkoutSessionCommand(Guid.NewGuid(), new DateTime(2025, 6, 15));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Any<WorkoutSession>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
