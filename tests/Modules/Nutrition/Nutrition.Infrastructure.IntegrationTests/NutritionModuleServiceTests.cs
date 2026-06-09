using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Nutrition.Domain.Entity;
using Nutrition.Domain.Enums;
using Nutrition.Domain.ValueObjects;
using Nutrition.Infrastructure.Persistence;
using Nutrition.Infrastructure.Services;
using Xunit;

namespace Nutrition.Infrastructure.IntegrationTests;

[Collection("SqlServer")]
public class NutritionModuleServiceTests : IAsyncLifetime
{
    private readonly NutritionDbContext _context;
    private readonly NutritionModuleService _sut;

    public NutritionModuleServiceTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<NutritionDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("NutritionModuleServiceTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new NutritionDbContext(options);
        _sut = new NutritionModuleService(_context);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task GetDailySummaryAsync_ShouldAggregateMacrosAcrossMealsInSql()
    {
        var userId = Guid.NewGuid();
        var date = new DateTime(2025, 6, 15);

        var mealPlan = MealPlan.Create(userId, "Plan", date, note: null);

        var breakfast = mealPlan.AddMeal("Breakfast", 1);
        mealPlan.AddItemToMeal(breakfast.Id, Guid.NewGuid(), "Eggs", 100m, ServingUnit.Gram,
            new MacroNutrients(150m, 13m, 1m, 11m));
        mealPlan.AddItemToMeal(breakfast.Id, Guid.NewGuid(), "Oats", 80m, ServingUnit.Gram,
            new MacroNutrients(300m, 10m, 50m, 6m));

        var lunch = mealPlan.AddMeal("Lunch", 2);
        mealPlan.AddItemToMeal(lunch.Id, Guid.NewGuid(), "Chicken", 200m, ServingUnit.Gram,
            new MacroNutrients(330m, 62m, 0m, 7m));

        await _context.MealPlans.AddAsync(mealPlan);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var summary = await _sut.GetDailySummaryAsync(userId, date);

        summary.Should().NotBeNull();
        summary!.TotalCalories.Should().Be(780m);
        summary.TotalProtein.Should().Be(85m);
        summary.TotalCarbohydrates.Should().Be(51m);
        summary.TotalFat.Should().Be(24m);
        summary.MealCount.Should().Be(2);
    }

    [Fact]
    public async Task GetDailySummaryAsync_ShouldReturnZeroTotals_WhenPlanHasNoItems()
    {
        var userId = Guid.NewGuid();
        var date = new DateTime(2025, 7, 1);

        var mealPlan = MealPlan.Create(userId, "Empty Plan", date, note: null);
        mealPlan.AddMeal("Breakfast", 1);

        await _context.MealPlans.AddAsync(mealPlan);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var summary = await _sut.GetDailySummaryAsync(userId, date);

        summary.Should().NotBeNull();
        summary!.TotalCalories.Should().Be(0m);
        summary.TotalProtein.Should().Be(0m);
        summary.MealCount.Should().Be(1);
    }

    [Fact]
    public async Task GetDailySummaryAsync_ShouldReturnNull_WhenNoPlanForDate()
    {
        var summary = await _sut.GetDailySummaryAsync(Guid.NewGuid(), new DateTime(2025, 8, 1));

        summary.Should().BeNull();
    }
}
