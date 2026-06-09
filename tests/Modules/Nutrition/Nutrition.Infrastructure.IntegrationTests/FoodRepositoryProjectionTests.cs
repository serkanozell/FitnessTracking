using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Nutrition.Application.Dtos;
using Nutrition.Domain.Entity;
using Nutrition.Domain.Enums;
using Nutrition.Infrastructure.Persistence;
using Nutrition.Infrastructure.Repositories;
using NSubstitute;
using Xunit;

namespace Nutrition.Infrastructure.IntegrationTests;

/// <summary>
/// Verifies the P3 SQL projection path for Foods against a real SQL Server.
/// EF Core InMemory provider does NOT translate to SQL, so this container-backed
/// test is the only place that proves FoodDto.Projection (including enum.ToString()
/// on HasConversion&lt;string&gt; columns and the owned Macros value object) is
/// SQL-translatable and returns correct values.
/// </summary>
[Collection("SqlServer")]
public class FoodRepositoryProjectionTests : IAsyncLifetime
{
    private readonly NutritionDbContext _context;
    private readonly FoodRepository _sut;
    private static readonly Guid TestUserId = Guid.NewGuid();

    public FoodRepositoryProjectionTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<NutritionDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("FoodProjectionTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new NutritionDbContext(options);
        _sut = new FoodRepository(_context);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();

    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task GetPagedAsync_WithProjection_ShouldTranslateToSqlAndReturnDtos()
    {
        // Arrange
        var food = Food.Create(
            name: "Chicken Breast",
            category: FoodCategory.Protein,
            defaultServingSize: 100m,
            servingUnit: ServingUnit.Gram,
            calories: 165m,
            protein: 31m,
            carbohydrates: 0m,
            fat: 3.6m,
            fiber: 0m,
            userId: TestUserId);

        await _sut.AddAsync(food);
        await _context.SaveChangesAsync();

        // Act - projection runs as IQueryable.Select against real SQL Server.
        // If enum.ToString() or the owned Macros members were not translatable,
        // this call would throw at query time.
        var (items, totalCount) = await _sut.GetPagedAsync(TestUserId, 1, 10, FoodDto.Projection);

        // Assert
        totalCount.Should().Be(1);
        items.Should().ContainSingle();

        var dto = items[0];
        dto.Id.Should().Be(food.Id);
        dto.Name.Should().Be("Chicken Breast");
        dto.Category.Should().Be(nameof(FoodCategory.Protein));
        dto.ServingUnit.Should().Be(nameof(ServingUnit.Gram));
        dto.Calories.Should().Be(165m);
        dto.Protein.Should().Be(31m);
        dto.Carbohydrates.Should().Be(0m);
        dto.Fat.Should().Be(3.6m);
        dto.UserId.Should().Be(TestUserId);
    }
}
