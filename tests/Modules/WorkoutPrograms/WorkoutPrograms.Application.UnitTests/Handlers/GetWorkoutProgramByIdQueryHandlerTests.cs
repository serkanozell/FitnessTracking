using FluentAssertions;
using BuildingBlocks.Application.Abstractions;
using NSubstitute;
using WorkoutPrograms.Application.Features.WorkoutPrograms.GetWorkoutProgramById;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.Repositories;
using Xunit;

namespace WorkoutPrograms.Application.UnitTests.Handlers;

public class GetWorkoutProgramByIdQueryHandlerTests
{
    private readonly IWorkoutProgramRepository _repository = Substitute.For<IWorkoutProgramRepository>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private readonly GetWorkoutProgramByIdQueryHandler _sut;

    public GetWorkoutProgramByIdQueryHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
        _sut = new GetWorkoutProgramByIdQueryHandler(_repository, _currentUser);
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenExists()
    {
        var program = WorkoutProgram.Create(TestUserId, "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var query = new GetWorkoutProgramByIdQuery(program.Id);
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(program);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Id.Should().Be(program.Id);
        result.Data.Name.Should().Be("PPL");
        result.Data.StartDate.Should().Be(new DateTime(2025, 1, 1));
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFoundError_WhenNotExists()
    {
        var query = new GetWorkoutProgramByIdQuery(Guid.NewGuid());
        _repository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((WorkoutProgram?)null);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("WorkoutProgram.NotFound");
    }
}
