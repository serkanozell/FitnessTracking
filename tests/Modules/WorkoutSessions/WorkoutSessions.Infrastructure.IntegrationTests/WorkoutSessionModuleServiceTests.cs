using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using NSubstitute;
using WorkoutPrograms.Contracts;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Infrastructure.Persistence;
using WorkoutSessions.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WorkoutSessions.Infrastructure.IntegrationTests;

[Collection("SqlServer")]
public class WorkoutSessionModuleServiceTests : IAsyncLifetime
{
    private readonly WorkoutSessionsDbContext _context;
    private readonly WorkoutSessionModuleService _sut;

    public WorkoutSessionModuleServiceTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<WorkoutSessionsDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("WorkoutSessionModuleServiceTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new WorkoutSessionsDbContext(options);
        _sut = new WorkoutSessionModuleService(_context, Substitute.For<IWorkoutProgramModule>());
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task GetStatsByUserAsync_ShouldAggregateSetsAndRepsInSql()
    {
        var userId = Guid.NewGuid();

        var s1 = WorkoutSession.Create(userId, Guid.NewGuid(), Guid.NewGuid(), new DateTime(2025, 6, 1));
        s1.Activate();
        s1.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        s1.AddEntry(Guid.NewGuid(), 2, 85m, 8);

        var s2 = WorkoutSession.Create(userId, Guid.NewGuid(), Guid.NewGuid(), new DateTime(2025, 6, 2));
        s2.Activate();
        s2.AddEntry(Guid.NewGuid(), 1, 100m, 5);

        // Different user — must be excluded.
        var other = WorkoutSession.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), new DateTime(2025, 6, 1));
        other.Activate();
        other.AddEntry(Guid.NewGuid(), 1, 50m, 20);

        await _context.WorkoutSessions.AddRangeAsync(s1, s2, other);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var stats = await _sut.GetStatsByUserAsync(userId, new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));

        stats.SessionCount.Should().Be(2);
        stats.TotalSets.Should().Be(3);
        stats.TotalReps.Should().Be(23);
    }

    [Fact]
    public async Task GetStatsByUserAsync_ShouldReturnZeroes_WhenNoSessionsInRange()
    {
        var userId = Guid.NewGuid();

        var session = WorkoutSession.Create(userId, Guid.NewGuid(), Guid.NewGuid(), new DateTime(2024, 1, 1));
        session.Activate();
        session.AddEntry(Guid.NewGuid(), 1, 60m, 12);
        await _context.WorkoutSessions.AddAsync(session);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var stats = await _sut.GetStatsByUserAsync(userId, new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));

        stats.SessionCount.Should().Be(0);
        stats.TotalSets.Should().Be(0);
        stats.TotalReps.Should().Be(0);
        stats.StreakDays.Should().Be(0);
    }
}
