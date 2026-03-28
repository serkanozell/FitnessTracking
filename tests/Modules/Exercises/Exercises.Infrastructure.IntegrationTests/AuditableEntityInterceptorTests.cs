using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Exercises.Infrastructure.IntegrationTests;

[Collection("SqlServer")]
public class AuditableEntityInterceptorTests : IAsyncLifetime
{
    private readonly ExercisesDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AuditableEntityInterceptorTests(SqlServerContainerFixture fixture)
    {
        _currentUser = Substitute.For<ICurrentUser>();
        _currentUser.IsAuthenticated.Returns(true);
        _currentUser.UserId.Returns("user-123");

        var interceptor = new AuditableEntityInterceptor(_currentUser);
        var options = new DbContextOptionsBuilder<ExercisesDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("AuditableTests"))
            .AddInterceptors(interceptor)
            .Options;

        _context = new ExercisesDbContext(options);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task SavingChanges_ShouldSetCreatedByAndCreatedDate_OnAdd()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, null, "");

        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var saved = await _context.Exercises.FindAsync(exercise.Id);
        saved!.CreatedBy.Should().NotBeNullOrEmpty();
        saved.CreatedDate.Should().NotBeNull();
        saved.IsActive.Should().BeTrue();
        saved.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task SavingChanges_ShouldSetUpdatedByAndUpdatedDate_OnModify()
    {
        var exercise = Exercise.Create("Old", MuscleGroup.Chest, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        exercise.Update("New", MuscleGroup.Back, null, "Updated", null, null);
        _context.Exercises.Update(exercise);
        await _context.SaveChangesAsync();

        var saved = await _context.Exercises.FindAsync(exercise.Id);
        saved!.UpdatedBy.Should().Be("user-123");
        saved.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task SavingChanges_ShouldSoftDelete_OnDelete()
    {
        var exercise = Exercise.Create("ToDelete", MuscleGroup.Chest, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        _context.Exercises.Remove(exercise);
        await _context.SaveChangesAsync();

        var saved = await _context.Exercises.FindAsync(exercise.Id);
        saved.Should().NotBeNull();
        saved!.IsActive.Should().BeFalse();
        saved.IsDeleted.Should().BeTrue();
        saved.UpdatedBy.Should().Be("user-123");
        saved.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public async Task SavingChanges_ShouldUseSystem_WhenUserNotAuthenticated()
    {
        _currentUser.IsAuthenticated.Returns(false);

        var exercise = Exercise.Create("Test", MuscleGroup.Chest, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var saved = await _context.Exercises.FindAsync(exercise.Id);
        saved!.CreatedBy.Should().Be("system");
    }
}
