using BodyMetrics.Domain.Entity;
using BodyMetrics.Infrastructure.Persistence;
using BodyMetrics.Infrastructure.Repositories;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace BodyMetrics.Infrastructure.IntegrationTests;

[Collection("SqlServer")]
public class BodyMetricRepositoryTests : IAsyncLifetime
{
    private readonly BodyMetricsDbContext _context;
    private readonly BodyMetricRepository _sut;
    private static readonly Guid TestUserId = Guid.NewGuid();

    public BodyMetricRepositoryTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<BodyMetricsDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("BodyMetricRepoTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new BodyMetricsDbContext(options);
        _sut = new BodyMetricRepository(_context);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    // --- AddAsync ---

    [Fact]
    public async Task AddAsync_ShouldPersistBodyMetric()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, 15m, 65m, 85m, 100m, 35m, 95m, 55m, 38m, null, "Test note");

        await _sut.AddAsync(metric);
        await _context.SaveChangesAsync();

        var saved = await _context.BodyMetrics.FindAsync(metric.Id);
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(TestUserId);
        saved.Weight.Should().Be(80m);
        saved.Height.Should().Be(180m);
        saved.BodyFatPercentage.Should().Be(15m);
        saved.Note.Should().Be("Test note");
    }

    [Fact]
    public async Task AddAsync_ShouldPersistWithNullOptionalFields()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);

        await _sut.AddAsync(metric);
        await _context.SaveChangesAsync();

        var saved = await _context.BodyMetrics.FindAsync(metric.Id);
        saved.Should().NotBeNull();
        saved!.Weight.Should().Be(80m);
        saved.Height.Should().BeNull();
        saved.BodyFatPercentage.Should().BeNull();
        saved.Note.Should().BeNull();
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBodyMetric_WhenExists()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, null, null, null, null, null, null, null, null, null, null);
        await _context.BodyMetrics.AddAsync(metric);
        await _context.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(metric.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(metric.Id);
        result.Weight.Should().Be(80m);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());
        result.Should().BeNull();
    }

    // --- GetActiveByUserIdAsync ---

    [Fact]
    public async Task GetActiveByUserIdAsync_ShouldReturnOnlyActiveMetrics()
    {
        var active1 = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        var active2 = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 15), 79m, null, null, null, null, null, null, null, null, null, null, null);
        var deleted = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 10), 81m, null, null, null, null, null, null, null, null, null, null, null);
        deleted.Delete();

        await _context.BodyMetrics.AddRangeAsync(active1, active2, deleted);
        await _context.SaveChangesAsync();

        var result = await _sut.GetActiveByUserIdAsync(TestUserId);

        result.Should().HaveCount(2);
        result.Should().NotContain(x => x.Id == deleted.Id);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_ShouldNotReturnOtherUsersMetrics()
    {
        var ownMetric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        var otherMetric = BodyMetric.Create(Guid.NewGuid(), new DateTime(2025, 6, 1), 75m, null, null, null, null, null, null, null, null, null, null, null);

        await _context.BodyMetrics.AddRangeAsync(ownMetric, otherMetric);
        await _context.SaveChangesAsync();

        var result = await _sut.GetActiveByUserIdAsync(TestUserId);

        result.Should().ContainSingle();
        result[0].Id.Should().Be(ownMetric.Id);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_ShouldReturnEmpty_WhenNoData()
    {
        var result = await _sut.GetActiveByUserIdAsync(Guid.NewGuid());
        result.Should().BeEmpty();
    }

    // --- GetPagedByUserAsync ---

    [Fact]
    public async Task GetPagedByUserAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 5; i++)
        {
            await _context.BodyMetrics.AddAsync(
                BodyMetric.Create(TestUserId, new DateTime(2025, 6, i), 80m + i, null, null, null, null, null, null, null, null, null, null, null));
        }
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _sut.GetPagedByUserAsync(TestUserId, 1, 2);

        items.Should().HaveCount(2);
        totalCount.Should().Be(5);
    }

    [Fact]
    public async Task GetPagedByUserAsync_ShouldReturnEmpty_WhenNoData()
    {
        var (items, totalCount) = await _sut.GetPagedByUserAsync(Guid.NewGuid(), 1, 10);

        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetPagedByUserAsync_ShouldOrderByDateDescending()
    {
        var older = BodyMetric.Create(TestUserId, new DateTime(2025, 1, 1), 82m, null, null, null, null, null, null, null, null, null, null, null);
        var newer = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 78m, null, null, null, null, null, null, null, null, null, null, null);

        await _context.BodyMetrics.AddRangeAsync(older, newer);
        await _context.SaveChangesAsync();

        var (items, _) = await _sut.GetPagedByUserAsync(TestUserId, 1, 10);

        items[0].Date.Should().BeAfter(items[1].Date);
    }

    // --- Update ---

    [Fact]
    public async Task Update_ShouldModifyBodyMetric()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, null, null, null, null, null, null, null, null, null, null);
        await _context.BodyMetrics.AddAsync(metric);
        await _context.SaveChangesAsync();

        metric.Update(new DateTime(2025, 7, 1), 78m, 180m, 14m, null, null, null, null, null, null, null, null, "Updated");
        _sut.Update(metric);
        await _context.SaveChangesAsync();

        var updated = await _context.BodyMetrics.FindAsync(metric.Id);
        updated!.Weight.Should().Be(78m);
        updated.BodyFatPercentage.Should().Be(14m);
        updated.Note.Should().Be("Updated");
    }
}

