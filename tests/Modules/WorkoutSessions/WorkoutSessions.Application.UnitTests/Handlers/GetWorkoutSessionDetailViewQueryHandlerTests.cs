using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using Exercises.Contracts;
using NSubstitute;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Application.Features.WorkoutSessions.GetWorkoutSessionDetailView;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Domain.Repositories;
using Xunit;

namespace WorkoutSessions.Application.UnitTests.Handlers;

public class GetWorkoutSessionDetailViewQueryHandlerTests
{
    private readonly IWorkoutSessionRepository _repository = Substitute.For<IWorkoutSessionRepository>();
    private readonly IWorkoutProgramModule _programModule = Substitute.For<IWorkoutProgramModule>();
    private readonly IExerciseModule _exerciseModule = Substitute.For<IExerciseModule>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly GetWorkoutSessionDetailViewQueryHandler _sut;

    public GetWorkoutSessionDetailViewQueryHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new GetWorkoutSessionDetailViewQueryHandler(
            _repository, _programModule, _exerciseModule, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenSessionNotExists()
    {
        var query = new GetWorkoutSessionDetailViewQuery(Guid.NewGuid());
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((WorkoutSession?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutSession.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnForbidden_WhenUserDoesNotOwnSession()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.Today);
        var query = new GetWorkoutSessionDetailViewQuery(session.Id);
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldAggregateProgramAndExerciseData_WhenSessionExists()
    {
        var programId = Guid.NewGuid();
        var splitId = Guid.NewGuid();
        var otherSplitId = Guid.NewGuid();
        var exerciseInSplitId = Guid.NewGuid();
        var deletedExerciseId = Guid.NewGuid();
        var sessionExerciseId = Guid.NewGuid();
        var unrelatedExerciseId = Guid.NewGuid();

        var session = WorkoutSession.Create(TestUserId, programId, splitId, new DateTime(2025, 6, 15));
        session.AddEntry(sessionExerciseId, 1, 80m, 10);

        var query = new GetWorkoutSessionDetailViewQuery(session.Id);
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(session);

        var program = new ProgramDetailInfo(
            programId,
            "Push/Pull/Legs",
            new List<ProgramSplitSummaryInfo>
            {
                new(splitId, "Push Day", 1, false, new List<ProgramSplitExerciseInfo>
                {
                    new(exerciseInSplitId, false),
                    new(deletedExerciseId, true)
                }),
                new(otherSplitId, "Pull Day", 2, false, new List<ProgramSplitExerciseInfo>
                {
                    new(unrelatedExerciseId, false)
                })
            });

        _programModule.GetProgramWithSplitsAsync(programId, Arg.Any<CancellationToken>()).Returns(program);

        _exerciseModule.GetExercisesAsync(Arg.Any<CancellationToken>()).Returns(new List<ExerciseInfo>
        {
            new(exerciseInSplitId, "Bench Press", "Chest", null, ""),
            new(deletedExerciseId, "Old Exercise", "Chest", null, ""),
            new(unrelatedExerciseId, "Row", "Back", null, ""),
            new(sessionExerciseId, "Squat", "Legs", null, "")
        });

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Session.Id.Should().Be(session.Id);
        result.Data.ProgramName.Should().Be("Push/Pull/Legs");

        result.Data.ProgramSplits.Should().HaveCount(2);
        result.Data.ProgramSplits.Select(s => s.Order).Should().BeInAscendingOrder();

        // Only the session's split exercises (excluding deleted) should be returned for the Add-Set dropdown.
        result.Data.SessionSplitExercises.Should().ContainSingle()
            .Which.ExerciseId.Should().Be(exerciseInSplitId);

        // Exercise names should only contain the ids actually referenced by the session.
        result.Data.ExerciseNames.Should().ContainKey(sessionExerciseId)
            .WhoseValue.Should().Be("Squat");
        result.Data.ExerciseNames.Should().NotContainKey(unrelatedExerciseId);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyAggregates_WhenProgramNotFound()
    {
        var session = WorkoutSession.Create(TestUserId, Guid.NewGuid(), Guid.NewGuid(), DateTime.Today);
        var query = new GetWorkoutSessionDetailViewQuery(session.Id);
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(session);
        _programModule.GetProgramWithSplitsAsync(session.WorkoutProgramId, Arg.Any<CancellationToken>())
            .Returns((ProgramDetailInfo?)null);
        _exerciseModule.GetExercisesAsync(Arg.Any<CancellationToken>()).Returns(new List<ExerciseInfo>());

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.ProgramName.Should().BeEmpty();
        result.Data.ProgramSplits.Should().BeEmpty();
        result.Data.SessionSplitExercises.Should().BeEmpty();
    }
}
