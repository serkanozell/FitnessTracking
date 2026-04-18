using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Application.Features.WorkoutSessions.CreateWorkoutSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class CreateWorkoutSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutProgramModule _programModule = Substitute.For<IWorkoutProgramModule>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly CreateWorkoutSessionCommandHandler _sut;
    private static readonly Guid TestUserId = Guid.NewGuid();

    public CreateWorkoutSessionCommandHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _programModule.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _programModule.IsOwnedByUserAsync(Arg.Any<Guid>(), TestUserId, Arg.Any<CancellationToken>()).Returns(true);
        _programModule.SplitBelongsToProgramAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        _sut = new CreateWorkoutSessionCommandHandler(_repository, _programModule, _unitOfWork, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldCreateSession()
    {
        var command = new CreateWorkoutSessionCommand(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2025, 6, 15));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Any<WorkoutSession>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnProgramNotFound_WhenProgramDoesNotExist()
    {
        _programModule.ExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);
        var command = new CreateWorkoutSessionCommand(Guid.NewGuid(), Guid.NewGuid(), new DateTime(2025, 6, 15));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.ProgramNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenUserDoesNotOwnProgram()
    {
        var programId = Guid.NewGuid();
        _programModule.IsOwnedByUserAsync(programId, TestUserId, Arg.Any<CancellationToken>()).Returns(false);
        var command = new CreateWorkoutSessionCommand(programId, Guid.NewGuid(), new DateTime(2025, 6, 15));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Error.Forbidden");
    }

    [Fact]
    public async Task Handle_ShouldAllowAdmin_WhenNotOwner()
    {
        var programId = Guid.NewGuid();
        _currentUser.IsAdmin.Returns(true);
        _programModule.IsOwnedByUserAsync(programId, TestUserId, Arg.Any<CancellationToken>()).Returns(false);
        var command = new CreateWorkoutSessionCommand(programId, Guid.NewGuid(), new DateTime(2025, 6, 15));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
