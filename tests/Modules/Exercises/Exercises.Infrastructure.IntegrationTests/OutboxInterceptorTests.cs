using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Outbox;
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
public class OutboxInterceptorTests : IAsyncLifetime
{
    private readonly ExercisesDbContext _context;

    public OutboxInterceptorTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(false);

        var options = new DbContextOptionsBuilder<ExercisesDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("OutboxTests"))
            .AddInterceptors(new OutboxInterceptor(), new AuditableEntityInterceptor(currentUser))
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
    public async Task SavingChanges_ShouldConvertDomainEventsToOutboxMessages()
    {
        var exercise = Exercise.Create("Bench Press", MuscleGroup.Chest, MuscleGroup.Triceps, "Flat bench");
        // Exercise.Create raises ExerciseCreatedEvent as a domain event

        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var outboxMessages = await _context.OutboxMessages.ToListAsync();
        outboxMessages.Should().NotBeEmpty();
        outboxMessages.Should().Contain(m => m.EventType!.Contains("ExerciseCreatedEvent"));
        outboxMessages.All(m => m.IsProcessed == false).Should().BeTrue();
    }

    [Fact]
    public async Task SavingChanges_ShouldClearDomainEventsAfterConversion()
    {
        var exercise = Exercise.Create("Squat", MuscleGroup.Quadriceps, null, "");

        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        exercise.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task SavingChanges_ShouldNotCreateOutboxMessages_WhenNoDomainEvents()
    {
        // Directly add to context without domain events
        var exercise = Exercise.Create("Deadlift", MuscleGroup.Back, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        // Clear existing messages from the Create event
        var initialCount = await _context.OutboxMessages.CountAsync();

        // Now do a simple update that doesn't raise events (if possible)
        // The SaveChanges should not add more outbox messages if no new events
        await _context.SaveChangesAsync();

        var afterCount = await _context.OutboxMessages.CountAsync();
        afterCount.Should().Be(initialCount);
    }

    [Fact]
    public async Task SavingChanges_ShouldSetCorrectOutboxMessageProperties()
    {
        var exercise = Exercise.Create("OHP", MuscleGroup.Shoulders, null, "");
        await _context.Exercises.AddAsync(exercise);
        await _context.SaveChangesAsync();

        var message = await _context.OutboxMessages.FirstAsync();

        message.Id.Should().NotBeEmpty();
        message.EventType.Should().NotBeNullOrEmpty();
        message.Content.Should().NotBeNullOrEmpty();
        message.IsProcessed.Should().BeFalse();
        message.OccurredOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        message.ProcessedOnUtc.Should().BeNull();
        message.Error.Should().BeNull();
    }
}
