using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WorkoutPrograms.Domain.Entity;
using WorkoutPrograms.Domain.ValueObjects;
using WorkoutPrograms.Infrastructure.Persistence;
using WorkoutPrograms.Infrastructure.Repositories;
using Xunit;

namespace WorkoutPrograms.Infrastructure.IntegrationTests;

[Collection("SqlServer")]
public class WorkoutProgramRepositoryTests : IAsyncLifetime
{
    private readonly WorkoutProgramsDbContext _context;
    private readonly WorkoutProgramRepository _sut;

    public WorkoutProgramRepositoryTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<WorkoutProgramsDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("WorkoutProgramRepoTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new WorkoutProgramsDbContext(options);
        _sut = new WorkoutProgramRepository(_context);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistProgramWithSplitsAndExercises()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "PPL", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Push Day", 1);
        program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 4, new RepRange(8, 12));

        await _sut.AddAsync(program);
        await _context.SaveChangesAsync();

        var saved = await _context.WorkoutPrograms
            .Include(p => p.Splits).ThenInclude(s => s.Exercises)
            .FirstAsync(p => p.Id == program.Id);

        saved.Name.Should().Be("PPL");
        saved.Splits.Should().ContainSingle();
        saved.Splits.First().Exercises.Should().ContainSingle();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProgramWithSplits()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "UL", new DateTime(2025, 1, 1), new DateTime(2025, 6, 30));
        program.AddSplit("Upper", 1);
        program.AddSplit("Lower", 2);
        await _context.WorkoutPrograms.AddAsync(program);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var result = await _sut.GetByIdAsync(program.Id);

        result.Should().NotBeNull();
        result!.Splits.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdWithExercisesAsync_ShouldIncludeNestedExercises()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "Full Body", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        var split = program.AddSplit("Day A", 1);
        program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 3, new RepRange(10, 15));
        program.AddExerciseToSplit(split.Id, Guid.NewGuid(), 4, new RepRange(6, 8));
        await _context.WorkoutPrograms.AddAsync(program);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var result = await _sut.GetByIdWithExercisesAsync(program.Id);

        result.Should().NotBeNull();
        result!.Splits.First().Exercises.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPageWithTotalCount()
    {
        for (int i = 1; i <= 5; i++)
        {
            var p = WorkoutProgram.Create(Guid.NewGuid(), $"Program {i}", new DateTime(2025, i, 1), new DateTime(2025, i + 1, 1));
            await _context.WorkoutPrograms.AddAsync(p);
        }
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _sut.GetPagedAsync(1, 3);

        items.Should().HaveCount(3);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyProgram()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "Old", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        await _context.WorkoutPrograms.AddAsync(program);
        await _context.SaveChangesAsync();

        program.Update("New", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        _sut.Update(program);
        await _context.SaveChangesAsync();

        var saved = await _context.WorkoutPrograms.FindAsync(program.Id);
        saved!.Name.Should().Be("New");
        saved.StartDate.Should().Be(new DateTime(2025, 4, 1));
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteProgram()
    {
        var program = WorkoutProgram.Create(Guid.NewGuid(), "ToDelete", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        await _context.WorkoutPrograms.AddAsync(program);
        await _context.SaveChangesAsync();

        await _sut.DeleteAsync(program.Id);
        await _context.SaveChangesAsync();

        var deleted = await _context.WorkoutPrograms.FindAsync(program.Id);
        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
        deleted.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnAllWithSplitsAndExercises()
    {
        var p1 = WorkoutProgram.Create(Guid.NewGuid(), "P1", new DateTime(2025, 1, 1), new DateTime(2025, 3, 31));
        p1.AddSplit("S1", 1);
        var p2 = WorkoutProgram.Create(Guid.NewGuid(), "P2", new DateTime(2025, 4, 1), new DateTime(2025, 6, 30));
        await _context.WorkoutPrograms.AddRangeAsync(p1, p2);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var result = await _sut.GetListAsync();

        result.Should().HaveCount(2);
        result.First(p => p.Name == "P1").Splits.Should().ContainSingle();
    }
}
