using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using Exercises.Contracts;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class GetWorkoutProgramListQueryHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly IExerciseModule _exerciseModule = Substitute.For<IExerciseModule>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly GetWorkoutProgramListQueryHandler _sut;

    public GetWorkoutProgramListQueryHandlerTests()
    {
        _currentUser.UserId.Returns(_userId.ToString());
        _exerciseModule.GetExercisesAsync(Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ExerciseInfo>());
        _sut = new GetWorkoutProgramListQueryHandler(_repository, _exerciseModule, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        var programs = new List<WorkoutProgram>
        {
            WorkoutProgram.Create(Guid.NewGuid(), "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31)),
            WorkoutProgram.Create(Guid.NewGuid(), "Upper Lower", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30))
        };
        var query = new GetWorkoutProgramListQuery(1, 10);
        _repository.GetPagedByUserAsync(_userId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutProgram>)programs, 2));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoPrograms()
    {
        var query = new GetWorkoutProgramListQuery(1, 10);
        _repository.GetPagedByUserAsync(_userId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutProgram>)new List<WorkoutProgram>(), 0));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }
}
