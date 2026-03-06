using Exercises.Application.Features.Exercises.DeleteExercise;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.Handlers;

public class DeleteExerciseCommandHandlerTests
{
    private readonly IExerciseRepository _exerciseRepository = Substitute.For<IExerciseRepository>();
    private readonly IExercisesUnitOfWork _unitOfWork = Substitute.For<IExercisesUnitOfWork>();
    private readonly DeleteExerciseCommandHandler _sut;

    public DeleteExerciseCommandHandlerTests()
    {
        _sut = new DeleteExerciseCommandHandler(_exerciseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldDeleteExercise_WhenExists()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "Desc");
        exercise.Activate();
        var command = new DeleteExerciseCommand(exercise.Id);
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        exercise.IsDeleted.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new DeleteExerciseCommand(Guid.NewGuid());
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyDeletedError_WhenAlreadyDeleted()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "Desc");
        exercise.Delete();
        var command = new DeleteExerciseCommand(exercise.Id);
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.AlreadyDeleted");
    }
}
