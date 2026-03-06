using Exercises.Application.Features.Exercises.GetAllExercises;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Exercises.Application.UnitTests.Handlers;

public class GetAllExercisesQueryHandlerTests
{
    private readonly IExerciseRepository _exerciseRepository = Substitute.For<IExerciseRepository>();
    private readonly GetAllExercisesQueryHandler _sut;

    public GetAllExercisesQueryHandlerTests()
    {
        _sut = new GetAllExercisesQueryHandler(_exerciseRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        var exercises = new List<Exercise>
        {
            Exercise.Create("Bench Press", MuscleGroup.Chest, MuscleGroup.Triceps, "Flat bench"),
            Exercise.Create("Squat", MuscleGroup.Quadriceps, MuscleGroup.Glutes, "Barbell squat")
        };
        var query = new GetAllExercisesQuery(1, 10);
        _exerciseRepository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Exercise>)exercises, 2));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.PageNumber.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
        result.Data.Items[0].Name.Should().Be("Bench Press");
        result.Data.Items[0].PrimaryMuscleGroup.Should().Be("Chest");
        result.Data.Items[1].Name.Should().Be("Squat");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoExercises()
    {
        var query = new GetAllExercisesQuery(1, 10);
        _exerciseRepository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Exercise>)new List<Exercise>(), 0));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_ShouldMapSecondaryMuscleGroupAsNull_WhenNotSet()
    {
        var exercises = new List<Exercise>
        {
            Exercise.Create("Plank", MuscleGroup.Core, null, "Core exercise")
        };
        var query = new GetAllExercisesQuery(1, 10);
        _exerciseRepository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<Exercise>)exercises, 1));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items[0].SecondaryMuscleGroup.Should().BeNull();
    }
}
