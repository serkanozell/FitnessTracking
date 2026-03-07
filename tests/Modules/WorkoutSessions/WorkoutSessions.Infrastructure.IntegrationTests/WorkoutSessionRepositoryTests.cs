using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkoutSessions.Domain.Entity;
using WorkoutSessions.Infrastructure.Persistence;
using WorkoutSessions.Infrastructure.Repositories;
using Xunit;

namespace WorkoutSessions.Infrastructure.IntegrationTests;

public class WorkoutSessionRepositoryTests : IDisposable
{
    private readonly WorkoutSessionsDbContext _context;
    private readonly WorkoutSessionRepository _sut;

    public WorkoutSessionRepositoryTests()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<WorkoutSessionsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new WorkoutSessionsDbContext(options);
        _sut = new WorkoutSessionRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_ShouldPersistSessionWithExercises()
    {
        var programId = Guid.NewGuid();
        var session = WorkoutSession.Create(programId, new DateTime(2025, 6, 15));
        session.AddEntry(Guid.NewGuid(), 1, 80m, 10);

        await _sut.AddAsync(session);
        await _context.SaveChangesAsync();

        var saved = await _context.WorkoutSessions
            .Include(s => s.SessionExercises)
            .FirstAsync(s => s.Id == session.Id);

        saved.WorkoutProgramId.Should().Be(programId);
        saved.SessionExercises.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSessionWithExercises()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        session.AddEntry(Guid.NewGuid(), 1, 60m, 12);
        session.AddEntry(Guid.NewGuid(), 2, 65m, 10);
        await _context.WorkoutSessions.AddAsync(session);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var result = await _sut.GetByIdAsync(session.Id);

        result.Should().NotBeNull();
        result!.SessionExercises.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
        {
            var s = WorkoutSession.Create(Guid.NewGuid(), new DateTime(2025, 6, i));
            await _context.WorkoutSessions.AddAsync(s);
        }
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _sut.GetPagedAsync(1, 3);

        items.Should().HaveCount(3);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetPagedByProgramAsync_ShouldFilterByProgramId()
    {
        var targetProgramId = Guid.NewGuid();
        var otherProgramId = Guid.NewGuid();

        await _context.WorkoutSessions.AddAsync(WorkoutSession.Create(targetProgramId, new DateTime(2025, 6, 1)));
        await _context.WorkoutSessions.AddAsync(WorkoutSession.Create(targetProgramId, new DateTime(2025, 6, 2)));
        await _context.WorkoutSessions.AddAsync(WorkoutSession.Create(otherProgramId, new DateTime(2025, 6, 3)));
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _sut.GetPagedByProgramAsync(targetProgramId, 1, 10);

        items.Should().HaveCount(2);
        totalCount.Should().Be(2);
        items.Should().OnlyContain(s => s.WorkoutProgramId == targetProgramId);
    }

    [Fact]
    public async Task GetListByProgramAsync_ShouldReturnOnlyMatchingSessions()
    {
        var targetId = Guid.NewGuid();
        await _context.WorkoutSessions.AddAsync(WorkoutSession.Create(targetId, new DateTime(2025, 6, 1)));
        await _context.WorkoutSessions.AddAsync(WorkoutSession.Create(Guid.NewGuid(), new DateTime(2025, 6, 2)));
        await _context.SaveChangesAsync();

        var result = await _sut.GetListByProgramAsync(targetId);

        result.Should().ContainSingle();
        result[0].WorkoutProgramId.Should().Be(targetId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifySession()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), new DateTime(2025, 6, 1));
        await _context.WorkoutSessions.AddAsync(session);
        await _context.SaveChangesAsync();

        session.UpdateDate(new DateTime(2025, 7, 1));
        await _sut.UpdateAsync(session);
        await _context.SaveChangesAsync();

        var saved = await _context.WorkoutSessions.FindAsync(session.Id);
        saved!.Date.Should().Be(new DateTime(2025, 7, 1));
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteSession()
    {
        var session = WorkoutSession.Create(Guid.NewGuid(), DateTime.UtcNow);
        await _context.WorkoutSessions.AddAsync(session);
        await _context.SaveChangesAsync();

        await _sut.DeleteAsync(session.Id);
        await _context.SaveChangesAsync();

        var deleted = await _context.WorkoutSessions.FindAsync(session.Id);
        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
        deleted.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllSessionsWithExercises()
    {
        var s1 = WorkoutSession.Create(Guid.NewGuid(), new DateTime(2025, 6, 1));
        s1.AddEntry(Guid.NewGuid(), 1, 80m, 10);
        var s2 = WorkoutSession.Create(Guid.NewGuid(), new DateTime(2025, 6, 2));
        await _context.WorkoutSessions.AddRangeAsync(s1, s2);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }
}
