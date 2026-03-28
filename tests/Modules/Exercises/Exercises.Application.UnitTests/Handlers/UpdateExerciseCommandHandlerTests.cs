using Exercises.Application.Features.Exercises.UpdateExercise;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.Handlers;

public class UpdateExerciseCommandHandlerTests
{
    private readonly IExerciseRepository _exerciseRepository = Substitute.For<IExerciseRepository>();
    private readonly IExercisesUnitOfWork _unitOfWork = Substitute.For<IExercisesUnitOfWork>();
    private readonly UpdateExerciseCommandHandler _sut;

    public UpdateExerciseCommandHandlerTests()
    {
        _sut = new UpdateExerciseCommandHandler(_exerciseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateExercise_WhenExists()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "Desc");
        var command = new UpdateExerciseCommand(exercise.Id, "Incline Press", "Shoulders", "Chest", "Incline", null, null);
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        exercise.Name.Should().Be("Incline Press");
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var command = new UpdateExerciseCommand(Guid.NewGuid(), "Name", "Chest", null, "Desc", null, null);
        _exerciseRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.NotFound");
    }
}
