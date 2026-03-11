using FluentAssertions;
using NSubstitute;
using WorkoutSessions.Application.Features.WorkoutSessions.SessionExercises.GetExercisesBySession;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class GetExercisesBySessionQueryHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly GetExercisesBySessionQueryHandler _sut;

    public GetExercisesBySessionQueryHandlerTests()
    {
        _sut = new GetExercisesBySessionQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_ShouldReturnExercises_WhenSessionExists()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        var exerciseId = Guid.NewGuid();
        session.AddEntry(exerciseId, 1, 80m, 10);
        session.AddEntry(exerciseId, 2, 85m, 8);
        var query = new GetExercisesBySessionQuery(session.Id);
        _repository.GetByIdAsync(query.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var query = new GetExercisesBySessionQuery(Guid.NewGuid());
        _repository.GetByIdAsync(query.WorkoutSessionId, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }
}
