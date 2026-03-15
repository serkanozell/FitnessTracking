using BodyMetrics.Domain.Entity;
using BodyMetrics.Domain.Events;
using FluentAssertions;
using Xunit;

namespace BodyMetrics.Domain.UnitTests;

public class BodyMetricTests
{
    private static readonly Guid TestUserId = Guid.NewGuid();

    private static BodyMetric CreateDefault() =>
        BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, 15m, 65m, 85m, 100m, 35m, 95m, 55m, 38m, "Initial measurement");

    // --- Create ---

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, 15m, 65m, 85m, 100m, 35m, 95m, 55m, 38m, "Test");

        metric.Id.Should().NotBeEmpty();
        metric.UserId.Should().Be(TestUserId);
        metric.Date.Should().Be(new DateTime(2025, 6, 1));
        metric.Weight.Should().Be(80m);
        metric.Height.Should().Be(180m);
        metric.BodyFatPercentage.Should().Be(15m);
        metric.MuscleMass.Should().Be(65m);
        metric.WaistCircumference.Should().Be(85m);
        metric.ChestCircumference.Should().Be(100m);
        metric.ArmCircumference.Should().Be(35m);
        metric.HipCircumference.Should().Be(95m);
        metric.ThighCircumference.Should().Be(55m);
        metric.NeckCircumference.Should().Be(38m);
        metric.Note.Should().Be("Test");
    }

    [Fact]
    public void Create_ShouldAllowNullOptionalFields()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null);

        metric.Weight.Should().Be(80m);
        metric.Height.Should().BeNull();
        metric.BodyFatPercentage.Should().BeNull();
        metric.Note.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldRaiseBodyMetricCreatedEvent()
    {
        var metric = CreateDefault();

        metric.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BodyMetricCreatedEvent>()
            .Which.Should().Match<BodyMetricCreatedEvent>(e =>
                e.BodyMetricId == metric.Id && e.UserId == TestUserId);
    }

    // --- Update ---

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var metric = CreateDefault();
        metric.ClearDomainEvents();

        metric.Update(new DateTime(2025, 7, 1), 78m, 180m, 14m, 66m, 83m, 101m, 36m, 94m, 56m, 38m, "Updated");

        metric.Date.Should().Be(new DateTime(2025, 7, 1));
        metric.Weight.Should().Be(78m);
        metric.BodyFatPercentage.Should().Be(14m);
        metric.Note.Should().Be("Updated");
    }

    [Fact]
    public void Update_ShouldRaiseBodyMetricUpdatedEvent()
    {
        var metric = CreateDefault();
        metric.ClearDomainEvents();

        metric.Update(new DateTime(2025, 7, 1), 78m, 180m, 14m, 66m, 83m, 101m, 36m, 94m, 56m, 38m, null);

        metric.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BodyMetricUpdatedEvent>();
    }

    // --- Delete ---

    [Fact]
    public void Delete_ShouldSetIsDeletedTrue()
    {
        var metric = CreateDefault();
        metric.ClearDomainEvents();

        metric.Delete();

        metric.IsDeleted.Should().BeTrue();
        metric.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Delete_ShouldRaiseBodyMetricDeletedEvent()
    {
        var metric = CreateDefault();
        metric.ClearDomainEvents();

        metric.Delete();

        metric.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BodyMetricDeletedEvent>();
    }

    // --- Activate ---

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        var metric = CreateDefault();
        metric.Delete();
        metric.ClearDomainEvents();

        metric.Activate();

        metric.IsActive.Should().BeTrue();
        metric.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldRaiseBodyMetricActivatedEvent()
    {
        var metric = CreateDefault();
        metric.Delete();
        metric.ClearDomainEvents();

        metric.Activate();

        metric.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<BodyMetricActivatedEvent>();
    }
}
