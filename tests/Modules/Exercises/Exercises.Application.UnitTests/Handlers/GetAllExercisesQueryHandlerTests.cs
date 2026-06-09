using System.Linq.Expressions;
using Exercises.Application.Dtos;
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
        var dtos = new List<ExerciseDto>
        {
            ExerciseDto.FromEntity(Exercise.Create("Bench Press", MuscleGroup.Chest, MuscleGroup.Triceps, "Flat bench")),
            ExerciseDto.FromEntity(Exercise.Create("Squat", MuscleGroup.Quadriceps, MuscleGroup.Glutes, "Barbell squat"))
        };
        var query = new GetAllExercisesQuery(1, 10);
        _exerciseRepository.GetPagedAsync(1, 10, Arg.Any<Expression<Func<Exercise, ExerciseDto>>>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<ExerciseDto>)dtos, 2));

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
        _exerciseRepository.GetPagedAsync(1, 10, Arg.Any<Expression<Func<Exercise, ExerciseDto>>>(), Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<ExerciseDto>)new List<ExerciseDto>(), 0));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public void Projection_ShouldMapMuscleGroupsToString()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, MuscleGroup.Triceps, "Flat bench");

        var dto = ExerciseDto.Projection.Compile().Invoke(exercise);

        dto.PrimaryMuscleGroup.Should().Be("Chest");
        dto.SecondaryMuscleGroup.Should().Be("Triceps");
    }

    [Fact]
    public void Projection_ShouldMapSecondaryMuscleGroupAsNull_WhenNotSet()
    {
        var exercise = Exercise.Create("Plank", MuscleGroup.Core, null, "Core exercise");

        var dto = ExerciseDto.Projection.Compile().Invoke(exercise);

        dto.SecondaryMuscleGroup.Should().BeNull();
    }
}
