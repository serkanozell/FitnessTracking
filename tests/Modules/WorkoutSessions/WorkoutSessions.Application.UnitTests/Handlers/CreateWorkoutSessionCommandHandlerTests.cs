using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Feature.WorkoutSessions.CreateWorkoutSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class CreateWorkoutSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly CreateWorkoutSessionCommandHandler _sut;

    public CreateWorkoutSessionCommandHandlerTests()
    {
        _sut = new CreateWorkoutSessionCommandHandler(_repository, _unitOfWork);
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
