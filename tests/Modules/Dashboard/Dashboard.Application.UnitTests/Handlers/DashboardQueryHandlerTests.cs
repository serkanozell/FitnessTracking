using BodyMetrics.Contracts;
using BuildingBlocks.Application.Abstractions;
using Dashboard.Application.Features.Dashboard.GetDashboard;
using Dashboard.Application.Features.Dashboard.GetWeightTrend;
using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Contracts;
using Xunit;

namespace Dashboard.Application.UnitTests.Handlers;

public class DashboardQueryHandlerTests
{
    private readonly IWorkoutProgramModule _programModule = Substitute.For<IWorkoutProgramModule>();
    private readonly IWorkoutSessionModule _sessionModule = Substitute.For<IWorkoutSessionModule>();
    private readonly IBodyMetricModule _bodyMetricModule = Substitute.For<IBodyMetricModule>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();

    public DashboardQueryHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
    }

    // --- GetDashboard ---

    [Fact]
    public async Task GetDashboard_ShouldReturnDashboard_WhenDataExists()
    {
        _programModule.GetActiveProgramByUserAsync(TestUserId, Arg.Any<CancellationToken>())
            .Returns(new ActiveProgramInfo(Guid.NewGuid(), "PPL", DateTime.Today.AddDays(-30), DateTime.Today.AddDays(60)));

        _sessionModule.GetStatsByUserAsync(TestUserId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new WorkoutSessionStatsInfo(4, 48, 480, 3));

        _bodyMetricModule.GetLatestByUserAsync(TestUserId, Arg.Any<CancellationToken>())
            .Returns(new LatestBodyMetricInfo(DateTime.Today, 80m, 15m, 65m));

        var sut = new GetDashboardQueryHandler(_programModule, _sessionModule, _bodyMetricModule, _currentUser);

        var result = await sut.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.ActiveProgram.Should().NotBeNull();
        result.Data.ActiveProgram!.Name.Should().Be("PPL");
        result.Data.WeeklyWorkouts.Completed.Should().Be(4);
        result.Data.LatestBodyMetric!.Weight.Should().Be(80m);
        result.Data.Stats.TotalWorkouts.Should().Be(4);
    }

    [Fact]
    public async Task GetDashboard_ShouldReturnNullActiveProgram_WhenNoProgramExists()
    {
        _programModule.GetActiveProgramByUserAsync(TestUserId, Arg.Any<CancellationToken>())
            .Returns((ActiveProgramInfo?)null);

        _sessionModule.GetStatsByUserAsync(TestUserId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new WorkoutSessionStatsInfo(0, 0, 0, 0));

        _bodyMetricModule.GetLatestByUserAsync(TestUserId, Arg.Any<CancellationToken>())
            .Returns((LatestBodyMetricInfo?)null);

        var sut = new GetDashboardQueryHandler(_programModule, _sessionModule, _bodyMetricModule, _currentUser);

        var result = await sut.Handle(new GetDashboardQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.ActiveProgram.Should().BeNull();
        result.Data.LatestBodyMetric.Should().BeNull();
        result.Data.Stats.TotalWorkouts.Should().Be(0);
    }

    // --- GetWeightTrend ---

    [Fact]
    public async Task GetWeightTrend_ShouldReturnTrendPoints()
    {
        var points = new List<WeightTrendPoint>
        {
            new(DateTime.Today.AddDays(-2), 81m),
            new(DateTime.Today.AddDays(-1), 80.5m),
            new(DateTime.Today, 80m)
        };

        _bodyMetricModule.GetWeightTrendAsync(TestUserId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(points);

        var sut = new GetWeightTrendQueryHandler(_bodyMetricModule, _currentUser);

        var result = await sut.Handle(new GetWeightTrendQuery(90), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
        result.Data![0].Weight.Should().Be(81m);
        result.Data[2].Weight.Should().Be(80m);
    }

    [Fact]
    public async Task GetWeightTrend_ShouldReturnEmpty_WhenNoData()
    {
        _bodyMetricModule.GetWeightTrendAsync(TestUserId, Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(new List<WeightTrendPoint>());

        var sut = new GetWeightTrendQueryHandler(_bodyMetricModule, _currentUser);

        var result = await sut.Handle(new GetWeightTrendQuery(30), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }
}
