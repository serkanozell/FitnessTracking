using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Infrastructure.Specifications;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Specifications;

public class SpecificationEvaluatorTests
{
    private sealed class Sample
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Score { get; set; }
    }

    private sealed class SampleDbContext : DbContext
    {
        public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options) { }
        public DbSet<Sample> Samples => Set<Sample>();
    }

    private sealed class ScoreAboveSpecification : Specification<Sample>
    {
        public ScoreAboveSpecification(int threshold)
            : base(x => x.Score > threshold)
        {
            ApplyOrderByDescending(x => x.Score);
            ApplyNoTracking();
        }
    }

    private sealed class EmptySpecification : Specification<Sample>
    {
    }

    private static SampleDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SampleDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new SampleDbContext(options);
        context.Samples.AddRange(
            new Sample { Id = 1, Name = "A", Score = 10 },
            new Sample { Id = 2, Name = "B", Score = 30 },
            new Sample { Id = 3, Name = "C", Score = 20 });
        context.SaveChanges();
        context.ChangeTracker.Clear();
        return context;
    }

    [Fact]
    public void GetQuery_ShouldApplyCriteria()
    {
        using var context = CreateContext();

        var result = SpecificationEvaluator.GetQuery(context.Samples, new ScoreAboveSpecification(15)).ToList();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(x => x.Score > 15);
    }

    [Fact]
    public void GetQuery_ShouldApplyOrderByDescending()
    {
        using var context = CreateContext();

        var result = SpecificationEvaluator.GetQuery(context.Samples, new ScoreAboveSpecification(0)).ToList();

        result.Select(x => x.Score).Should().ContainInOrder(30, 20, 10);
    }

    [Fact]
    public void GetQuery_ShouldReturnAll_WhenSpecificationHasNoCriteria()
    {
        using var context = CreateContext();

        var result = SpecificationEvaluator.GetQuery(context.Samples, new EmptySpecification()).ToList();

        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task ToPagedListAsync_ShouldReturnPagedItemsAndTotalCount()
    {
        using var context = CreateContext();

        var (items, totalCount) = await context.Samples
            .ToPagedListAsync(new ScoreAboveSpecification(0), pageNumber: 1, pageSize: 2);

        totalCount.Should().Be(3);
        items.Should().HaveCount(2);
        items.Select(x => x.Score).Should().ContainInOrder(30, 20);
    }

    [Fact]
    public async Task ToListAsync_ShouldApplySpecification()
    {
        using var context = CreateContext();

        var result = await context.Samples.ToListAsync(new ScoreAboveSpecification(15));

        result.Should().HaveCount(2);
        result.Select(x => x.Score).Should().ContainInOrder(30, 20);
    }
}
