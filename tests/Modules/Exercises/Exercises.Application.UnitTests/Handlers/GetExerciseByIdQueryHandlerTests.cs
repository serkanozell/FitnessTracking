using Exercises.Application.Dtos;
using Exercises.Application.Features.Exercises.GetExerciseById;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.Handlers;

public class GetExerciseByIdQueryHandlerTests
{
    private readonly IExerciseRepository _exerciseRepository = Substitute.For<IExerciseRepository>();
    private readonly GetExerciseByIdQueryHandler _sut;

    public GetExerciseByIdQueryHandlerTests()
    {
        _sut = new GetExerciseByIdQueryHandler(_exerciseRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnExerciseDto_WhenExists()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, MuscleGroup.Triceps, "Flat bench");
        var query = new GetExerciseByIdQuery(exercise.Id);
        _exerciseRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns(exercise);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(exercise.Id);
        result.Data.Name.Should().Be("Bench Press");
        result.Data.PrimaryMuscleGroup.Should().Be("Chest");
        result.Data.SecondaryMuscleGroup.Should().Be("Triceps");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var query = new GetExerciseByIdQuery(Guid.NewGuid());
        _exerciseRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>())
            .Returns((Exercise?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Exercise.NotFound");
    }
}
