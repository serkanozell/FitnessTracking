using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessions;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class GetWorkoutSessionsQueryHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly GetWorkoutSessionsQueryHandler _sut;

    public GetWorkoutSessionsQueryHandlerTests()
    {
        _sut = new GetWorkoutSessionsQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenNoProgramFilter()
    {
        var sessions = new List<WorkoutSession>
        {
            WorkoutSession.Create(Guid.NewGuid(), DateTime.Now),
            WorkoutSession.Create(Guid.NewGuid(), DateTime.Now)
        };
        var query = new GetWorkoutSessionsQuery(null, 1, 10);
        _repository.GetPagedAsync(1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutSession>)sessions, 2));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        await _repository.DidNotReceive().GetPagedByProgramAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldFilterByProgram_WhenProgramIdProvided()
    {
        var programId = Guid.NewGuid();
        var sessions = new List<WorkoutSession> { WorkoutSession.Create(programId, DateTime.Now) };
        var query = new GetWorkoutSessionsQuery(programId, 1, 10);
        _repository.GetPagedByProgramAsync(programId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutSession>)sessions, 1));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().ContainSingle();
        await _repository.DidNotReceive().GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }
}
