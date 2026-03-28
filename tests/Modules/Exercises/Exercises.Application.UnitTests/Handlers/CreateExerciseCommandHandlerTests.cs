using Exercises.Application.Features.Exercises.CreateExercise;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.Handlers;

public class CreateExerciseCommandHandlerTests
{
    private readonly IExerciseRepository _exerciseRepository = Substitute.For<IExerciseRepository>();
    private readonly IExercisesUnitOfWork _unitOfWork = Substitute.For<IExercisesUnitOfWork>();
    private readonly CreateExerciseCommandHandler _sut;

    public CreateExerciseCommandHandlerTests()
    {
        _sut = new CreateExerciseCommandHandler(_exerciseRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateExercise_WhenNameIsUnique()
    {
        var command = new CreateExerciseCommand("Bench Press", "Chest", "Triceps", "Flat bench press", null, null);
        _exerciseRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _exerciseRepository.Received(1).AddAsync(Arg.Any<Exercise>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnDuplicateNameError_WhenNameExists()
    {
        var existingExercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "Desc");
        var command = new CreateExerciseCommand("Bench Press", "Chest", null, "Desc", null, null);
        _exerciseRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns(existingExercise);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.DuplicateName");
        await _exerciseRepository.DidNotReceive().AddAsync(Arg.Any<Exercise>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldParseSecondaryMuscleGroupAsNull_WhenNotProvided()
    {
        var command = new CreateExerciseCommand("Squat", "Quadriceps", null, "Barbell squat", null, null);
        _exerciseRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _exerciseRepository.Received(1).AddAsync(
            Arg.Is<Exercise>(e => e.SecondaryMuscleGroup == null),
            Arg.Any<CancellationToken>());
    }
}
