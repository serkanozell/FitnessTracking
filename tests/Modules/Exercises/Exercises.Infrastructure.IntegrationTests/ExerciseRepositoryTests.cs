using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using Exercises.Domain.Entity;
using Exercises.Domain.Enums;
using Exercises.Infrastructure.Persistence;
using Exercises.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace Exercises.Infrastructure.IntegrationTests;

public class ExerciseRepositoryTests : IDisposable
{
    private readonly ExercisesDbContext _context;
    private readonly ExerciseRepository _sut;

    public ExerciseRepositoryTests()
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<ExercisesDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new ExercisesDbContext(options);
        _sut = new ExerciseRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task AddAsync_ShouldPersistExercise()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, MuscleGroup.Triceps, "Flat bench");

        await _sut.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var saved = await _context.Exercises.FindAsync(exercise.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Bench Press");
        saved.PrimaryMuscleGroup.Should().Be(MuscleGroup.Chest);
        saved.SecondaryMuscleGroup.Should().Be(MuscleGroup.Triceps);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnExercise_WhenExists()
    {
        var exercise = Exercise.Create("Squat", MuscleGroup.Quadriceps, MuscleGroup.Glutes, "Barbell squat");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(exercise.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(exercise.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnExercise_WhenNameMatches()
    {
        var exercise = Exercise.Create("Deadlift", MuscleGroup.Back, null, "Conventional");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByNameAsync("Deadlift");

        result.Should().NotBeNull();
        result!.Name.Should().Be("Deadlift");
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenNameNotFound()
    {
        var result = await _sut.GetByNameAsync("NonExistent");
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllExercises()
    {
        await _context.Exercises.AddAsync(Exercise.Create("A", MuscleGroup.Chest, null, ""));
        await _context.Exercises.AddAsync(Exercise.Create("B", MuscleGroup.Back, null, ""));
        await _context.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
            await _context.Exercises.AddAsync(Exercise.Create($"Exercise {i}", MuscleGroup.Chest, null, ""));
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _sut.GetPagedAsync(1, 2);

        items.Should().HaveCount(2);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnEmptyList_WhenNoData()
    {
        var (items, totalCount) = await _sut.GetPagedAsync(1, 10);

        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExercise()
    {
        var exercise = Exercise.Create("Old Name", MuscleGroup.Chest, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        exercise.Update("New Name", MuscleGroup.Back, MuscleGroup.Biceps, "Updated desc");
        _sut.Update(exercise);
        await _context.SaveChangesAsync();

        var updated = await _context.Exercises.FindAsync(exercise.Id);
        updated!.Name.Should().Be("New Name");
        updated.PrimaryMuscleGroup.Should().Be(MuscleGroup.Back);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteExercise()
    {
        var exercise = Exercise.Create("To Delete", MuscleGroup.Chest, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        _sut.Delete(exercise);
        await _context.SaveChangesAsync();

        var deleted = await _context.Exercises.FindAsync(exercise.Id);
        deleted.Should().NotBeNull();
        deleted!.IsDeleted.Should().BeTrue();
        deleted.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        var exercise = Exercise.Create("Exists", MuscleGroup.Chest, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var result = await _sut.ExistsAsync(exercise.Id);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _sut.ExistsAsync(Guid.NewGuid());
        result.Should().BeFalse();
    }
}
