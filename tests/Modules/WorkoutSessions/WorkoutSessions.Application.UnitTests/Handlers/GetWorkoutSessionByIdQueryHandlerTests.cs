using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionById;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class GetWorkoutSessionByIdQueryHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly GetWorkoutSessionByIdQueryHandler _sut;

    public GetWorkoutSessionByIdQueryHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new GetWorkoutSessionByIdQueryHandler(_repository, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnDetailDto_WhenExists()
    {
        var programId = Guid.NewGuid();
        var session = WorkoutSession.Create(TestUserId, programId, Guid.NewGuid(), new DateTime(2025, 6, 15));
        var query = new GetWorkoutSessionByIdQuery(session.Id);
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(session.Id);
        result.Data.WorkoutProgramId.Should().Be(programId);
        result.Data.Date.Should().Be(new DateTime(2025, 6, 15));
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var query = new GetWorkoutSessionByIdQuery(Guid.NewGuid());
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }
}
