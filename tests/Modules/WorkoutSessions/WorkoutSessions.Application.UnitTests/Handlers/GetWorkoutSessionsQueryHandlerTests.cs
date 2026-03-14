using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessions;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class GetWorkoutSessionsQueryHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly GetWorkoutSessionsQueryHandler _sut;

    public GetWorkoutSessionsQueryHandlerTests()
    {
        _currentUser.UserId.Returns(_userId.ToString());
        _sut = new GetWorkoutSessionsQueryHandler(_repository, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResult_WhenNoProgramFilter()
    {
        var sessions = new List<WorkoutSession>
        {
            WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now),
            WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now)
        };
        var query = new GetWorkoutSessionsQuery(null, 1, 10);
        _repository.GetPagedByUserAsync(_userId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutSession>)sessions, 2));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_ShouldFilterByProgram_WhenProgramIdProvided()
    {
        var programId = Guid.NewGuid();
        var sessions = new List<WorkoutSession> { WorkoutSession.Create(Guid.NewGuid(), programId, DateTime.Now) };
        var query = new GetWorkoutSessionsQuery(programId, 1, 10);
        _repository.GetPagedByUserAndProgramAsync(_userId, programId, 1, 10, Arg.Any<CancellationToken>())
            .Returns(((IReadOnlyList<WorkoutSession>)sessions, 1));

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().ContainSingle();
    }
}
