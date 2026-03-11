using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionById;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class GetWorkoutSessionByIdQueryHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly GetWorkoutSessionByIdQueryHandler _sut;

    public GetWorkoutSessionByIdQueryHandlerTests()
    {
        _sut = new GetWorkoutSessionByIdQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnDetailDto_WhenExists()
    {
        var programId = Guid.NewGuid();
        var session = WorkoutSession.Create(programId, new DateTime(2025, 6, 15));
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
