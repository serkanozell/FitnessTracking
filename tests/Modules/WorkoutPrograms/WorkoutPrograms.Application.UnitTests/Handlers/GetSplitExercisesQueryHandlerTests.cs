using Exercises.Contracts;
using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.WorkoutProgramSplits.WorkoutProgramSplitExercises.GetSplitExercises;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.ValueObjects;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class GetSplitExercisesQueryHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly IExerciseModule _exerciseModule = Substitute.For<IExerciseModule>();
    private readonly GetSplitExercisesQueryHandler _sut;

    public GetSplitExercisesQueryHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new GetSplitExercisesQueryHandler(_repository, _exerciseModule, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnExercisesWithNames_WhenProgramAndSplitExist()
    {
        var program = WorkoutProgram.Create(TestUserId, "PPL", null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        var exerciseId = Guid.NewGuid();
        program.AddExerciseToSplit(split.Id, exerciseId, 4, new RepRange(8, 12));

        var query = new GetSplitExercisesQuery(program.Id, split.Id);
        _repository.GetByIdWithExercisesAsync(query.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);
        _exerciseModule.GetExercisesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<ExerciseInfo>
            {
                new(exerciseId, "Bench Press", "Chest", "Triceps", "Flat bench")
            });

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().ContainSingle();
        result.Data![0].ExerciseId.Should().Be(exerciseId);
        result.Data[0].ExerciseName.Should().Be("Bench Press");
        result.Data[0].Sets.Should().Be(4);
        result.Data[0].MinimumReps.Should().Be(8);
        result.Data[0].MaximumReps.Should().Be(12);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyName_WhenExerciseNotFoundInModule()
    {
        var program = WorkoutProgram.Create(TestUserId, "PPL", null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 4, new RepRange(8, 12));

        var query = new GetSplitExercisesQuery(program.Id, split.Id);
        _repository.GetByIdWithExercisesAsync(query.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);
        _exerciseModule.GetExercisesAsync(Arg.Any<CancellationToken>())
            .Returns(new List<ExerciseInfo>());

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data![0].ExerciseName.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenProgramNotExists()
    {
        var query = new GetSplitExercisesQuery(Guid.NewGuid(), Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(query.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnSplitNotFoundError_WhenSplitNotExists()
    {
        var program = WorkoutProgram.Create(TestUserId, "PPL", null, new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var query = new GetSplitExercisesQuery(program.Id, Guid.NewGuid());
        _repository.GetByIdWithExercisesAsync(query.WorkoutProgramId, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.SplitNotFound");
    }
}
