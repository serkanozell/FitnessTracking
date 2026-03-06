using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.ActivateWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.AddWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.DeleteWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.GetWorkoutProgramSplits;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.UpdateWorkoutProgramSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.ActivateSplitExercise;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.AddExerciseToSplit;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.RemoveSplitExercise;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.UpdateSplitExercise;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class AddWorkoutProgramSplitCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly AddWorkoutProgramSplitCommandHandler _sut;

    public AddWorkoutProgramSplitCommandHandlerTests()
    {
        _sut = new AddWorkoutProgramSplitCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAddSplit_WhenProgramExistsAndNameIsUnique()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new AddWorkoutProgramSplitCommand(program.Id, "Push Day", 1);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        program.Splits.Should().ContainSingle(s => s.Name == "Push Day");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new AddWorkoutProgramSplitCommand(Guid.NewGuid(), "Push Day", 1);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnDuplicateNameError_WhenSplitNameExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.AddSplit("Push Day", 1);
        var command = new AddWorkoutProgramSplitCommand(program.Id, "Push Day", 2);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitDuplicateName");
    }
}

public class DeleteWorkoutProgramSplitCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly DeleteWorkoutProgramSplitCommandHandler _sut;

    public DeleteWorkoutProgramSplitCommandHandlerTests()
    {
        _sut = new DeleteWorkoutProgramSplitCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldRemoveSplit_WhenProgramAndSplitExist()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var command = new DeleteWorkoutProgramSplitCommand(program.Id, split.Id);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        program.Splits.Should().BeEmpty();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new DeleteWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new DeleteWorkoutProgramSplitCommand(program.Id, Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }
}

public class UpdateWorkoutProgramSplitCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly UpdateWorkoutProgramSplitCommandHandler _sut;

    public UpdateWorkoutProgramSplitCommandHandlerTests()
    {
        _sut = new UpdateWorkoutProgramSplitCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateSplit_WhenProgramAndSplitExist()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var command = new UpdateWorkoutProgramSplitCommand(program.Id, split.Id, "Pull Day", 2);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        split.Name.Should().Be("Pull Day");
        split.Order.Should().Be(2);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new UpdateWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid(), "Pull Day", 2);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new UpdateWorkoutProgramSplitCommand(program.Id, Guid.NewGuid(), "Pull Day", 2);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }
}

public class ActivateWorkoutProgramSplitCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly ActivateWorkoutProgramSplitCommandHandler _sut;

    public ActivateWorkoutProgramSplitCommandHandlerTests()
    {
        _sut = new ActivateWorkoutProgramSplitCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateSplit_WhenProgramActiveAndSplitExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.Activate();
        var split = program.AddSplit("Push Day", 1);
        var command = new ActivateWorkoutProgramSplitCommand(program.Id, split.Id);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(split.Id);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new ActivateWorkoutProgramSplitCommand(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotActiveError_WhenProgramNotActive()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var command = new ActivateWorkoutProgramSplitCommand(program.Id, split.Id);
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotActive");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.Activate();
        var command = new ActivateWorkoutProgramSplitCommand(program.Id, Guid.NewGuid());
        _repository.GetByIdAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }
}

public class GetWorkoutProgramSplitsQueryHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly GetWorkoutProgramSplitsQueryHandler _sut;

    public GetWorkoutProgramSplitsQueryHandlerTests()
    {
        _sut = new GetWorkoutProgramSplitsQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitsOrderedByOrder_WhenProgramExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.AddSplit("Leg Day", 3);
        program.AddSplit("Push Day", 1);
        program.AddSplit("Pull Day", 2);
        var query = new GetWorkoutProgramSplitsQuery(program.Id);
        _repository.GetByIdAsync(query.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        result.Data![0].Name.Should().Be("Push Day");
        result.Data[1].Name.Should().Be("Pull Day");
        result.Data[2].Name.Should().Be("Leg Day");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var query = new GetWorkoutProgramSplitsQuery(Guid.NewGuid());
        _repository.GetByIdAsync(query.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }
}

public class AddExerciseToSplitCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly AddExerciseToSplitCommandHandler _sut;

    public AddExerciseToSplitCommandHandlerTests()
    {
        _sut = new AddExerciseToSplitCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAddExercise_WhenProgramAndSplitExist()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var exerciseId = Guid.NewGuid();
        var command = new AddExerciseToSplitCommand(program.Id, split.Id, exerciseId, 4, 8, 12);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        split.Exercises.Should().ContainSingle(e => e.ExerciseId == exerciseId);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new AddExerciseToSplitCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 4, 8, 12);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new AddExerciseToSplitCommand(program.Id, Guid.NewGuid(), Guid.NewGuid(), 4, 8, 12);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }
}

public class UpdateSplitExerciseCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly UpdateSplitExerciseCommandHandler _sut;

    public UpdateSplitExerciseCommandHandlerTests()
    {
        _sut = new UpdateSplitExerciseCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExercise_WhenAllExist()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var exercise = program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 4, 8, 12);
        var command = new UpdateSplitExerciseCommand(program.Id, split.Id, exercise.Id, 5, 6, 10);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        exercise.Sets.Should().Be(5);
        exercise.MinimumReps.Should().Be(6);
        exercise.MaximumReps.Should().Be(10);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new UpdateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5, 6, 10);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new UpdateSplitExerciseCommand(program.Id, Guid.NewGuid(), Guid.NewGuid(), 5, 6, 10);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotFoundError_WhenExerciseNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var command = new UpdateSplitExerciseCommand(program.Id, split.Id, Guid.NewGuid(), 5, 6, 10);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.ExerciseNotFoundInSplit");
    }
}

public class RemoveSplitExerciseCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly RemoveSplitExerciseCommandHandler _sut;

    public RemoveSplitExerciseCommandHandlerTests()
    {
        _sut = new RemoveSplitExerciseCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldRemoveExercise_WhenAllExist()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var exercise = program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 4, 8, 12);
        var command = new RemoveSplitExerciseCommand(program.Id, split.Id, exercise.Id);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        split.Exercises.Should().BeEmpty();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new RemoveSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new RemoveSplitExerciseCommand(program.Id, Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotFoundError_WhenExerciseNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var command = new RemoveSplitExerciseCommand(program.Id, split.Id, Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.ExerciseNotFoundInSplit");
    }
}

public class ActivateSplitExerciseCommandHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IWorkoutProgramsUnitOfWork _unitOfWork = Substitute.For<IWorkoutProgramsUnitOfWork>();
    private readonly ActivateSplitExerciseCommandHandler _sut;

    public ActivateSplitExerciseCommandHandlerTests()
    {
        _sut = new ActivateSplitExerciseCommandHandler(_repository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldActivateExercise_WhenAllConditionsMet()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.Activate();
        var split = program.AddSplit("Push Day", 1);
        program.ActivateSplit(split.Id);
        var exercise = program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 4, 8, 12);
        var command = new ActivateSplitExerciseCommand(program.Id, split.Id, exercise.Id);
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(exercise.Id);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var command = new ActivateSplitExerciseCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotActiveError_WhenProgramNotActive()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var command = new ActivateSplitExerciseCommand(program.Id, Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotActive");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.Activate();
        var command = new ActivateSplitExerciseCommand(program.Id, Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseNotFoundError_WhenExerciseNotExists()
    {
        var program = WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        program.Activate();
        var split = program.AddSplit("Push Day", 1);
        var command = new ActivateSplitExerciseCommand(program.Id, split.Id, Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(command.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.ExerciseNotFoundInSplit");
    }
}
