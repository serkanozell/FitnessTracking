using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramList;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class GetWorkoutProgramListQueryHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly GetWorkoutProgramListQueryHandler _sut;

    public GetWorkoutProgramListQueryHandlerTests()
    {
        _sut = new GetWorkoutProgramListQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult()
    {
        var programs = new List<WorkoutProgram>
        {
            WorkoutProgram.Create("PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31)),
            WorkoutProgram.Create("Upper Lower", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30))
        };
        var query = new GetWorkoutProgramListQuery(1, 10);
        _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
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
        _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutProgram>)new List<WorkoutProgram>(), 0));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }
}
