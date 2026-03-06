using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Application.Feature.WorkoutSessions.SessionExercises.AddExerciseToSession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class AddExerciseToSessionCommandHandlerTests
{
    private readonly IWorkoutSessionRepository _sessionRepository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutProgramModule _programModule = Substitute.For<IWorkoutProgramModule>();
    private readonly IWorkoutSessionsUnitOfWork _unitOfWork = Substitute.For<IWorkoutSessionsUnitOfWork>();
    private readonly AddExerciseToSessionCommandHandler _sut;

    public AddExerciseToSessionCommandHandlerTests()
    {
        _sut = new AddExerciseToSessionCommandHandler(_sessionRepository, _programModule, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAddExercise_WhenAllValidationsPass()
    {
        var programId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var session = WorkoutSession.Create(programId, DateTime.UtcNow);
        var command = new AddExerciseToSessionCommand(session.Id, exerciseId, 1, 80m, 10);

        _sessionRepository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);
        _programModule.ExistsAsync(programId, Arg.Any<CancellationToken>()).Returns(true);
        _programModule.GetProgramExerciseAsync(programId, exerciseId, Arg.Any<CancellationToken>())
            .Returns(new ProgramExerciseInfo(exerciseId, 4));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        session.SessionExercises.Should().ContainSingle();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var command = new AddExerciseToSessionCommand(Guid.NewGuid(), Guid.NewGuid(), 1, 80m, 10);
        _sessionRepository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnProgramNotFoundError_WhenProgramNotExists()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        var command = new AddExerciseToSessionCommand(session.Id, Guid.NewGuid(), 1, 80m, 10);
        _sessionRepository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);
        _programModule.ExistsAsync(session.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.ProgramNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotInProgramError_WhenExerciseNotInProgram()
    {
        var programId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var session = WorkoutSession.Create(programId, DateTime.UtcNow);
        var command = new AddExerciseToSessionCommand(session.Id, exerciseId, 1, 80m, 10);

        _sessionRepository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);
        _programModule.ExistsAsync(programId, Arg.Any<CancellationToken>()).Returns(true);
        _programModule.GetProgramExerciseAsync(programId, exerciseId, Arg.Any<CancellationToken>())
            .Returns((ProgramExerciseInfo?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.ExerciseNotInProgram");
    }

    [Fact]
    public async Task Handle_ShouldReturnSetLimitExceededError_WhenMaxSetsReached()
    {
        var programId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var session = WorkoutSession.Create(programId, DateTime.UtcNow);
        session.AddEntry(exerciseId, 1, 80m, 10);
        session.AddEntry(exerciseId, 2, 80m, 10);
        var command = new AddExerciseToSessionCommand(session.Id, exerciseId, 3, 80m, 10);

        _sessionRepository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);
        _programModule.ExistsAsync(programId, Arg.Any<CancellationToken>()).Returns(true);
        _programModule.GetProgramExerciseAsync(programId, exerciseId, Arg.Any<CancellationToken>())
            .Returns(new ProgramExerciseInfo(exerciseId, 2));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.SetLimitExceeded");
    }

    [Fact]
    public async Task Handle_ShouldReturnDuplicateSetError_WhenSetAlreadyExists()
    {
        var programId = Guid.NewGuid();
        var exerciseId = Guid.NewGuid();
        var session = WorkoutSession.Create(programId, DateTime.UtcNow);
        session.AddEntry(exerciseId, 1, 80m, 10);
        var command = new AddExerciseToSessionCommand(session.Id, exerciseId, 1, 90m, 8);

        _sessionRepository.GetByIdAsync(command.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);
        _programModule.ExistsAsync(programId, Arg.Any<CancellationToken>()).Returns(true);
        _programModule.GetProgramExerciseAsync(programId, exerciseId, Arg.Any<CancellationToken>())
            .Returns(new ProgramExerciseInfo(exerciseId, 4));

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.DuplicateSet");
    }
}
