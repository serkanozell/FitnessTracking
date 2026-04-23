using BodyMetrics.Application.Features.BodyMetrics.CreateBodyMetric;
using BodyMetrics.Application.Features.BodyMetrics.UpdateBodyMetric;
using BodyMetrics.Application.Features.BodyMetrics.DeleteBodyMetric;
using BodyMetrics.Application.Features.BodyMetrics.ActivateBodyMetric;
using BodyMetrics.Application.Features.BodyMetrics.GetBodyMetricById;
using BodyMetrics.Domain.Entity;
using BodyMetrics.Domain.Repositories;
using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BodyMetrics.Application.UnitTests.Handlers;

public class BodyMetricCommandHandlerTests
{
    private readonly IBodyMetricRepository _repository = Substitute.For<IBodyMetricRepository>();
    private readonly IBodyMetricsUnitOfWork _unitOfWork = Substitute.For<IBodyMetricsUnitOfWork>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();
    private static readonly Guid TestUserId = Guid.NewGuid();

    public BodyMetricCommandHandlerTests()
    {
        _currentUser.UserId.Returns(TestUserId.ToString());
        _currentUser.IsAuthenticated.Returns(true);
    }

    // --- Create ---

    [Fact]
    public async Task Create_ShouldReturnId_WhenValid()
    {
        var sut = new CreateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);
        var command = new CreateBodyMetricCommand(new DateTime(2025, 6, 1), 80m, 180m, 15m, null, null, null, null, null, null, null, null, "Note");

        var result = await sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _repository.Received(1).AddAsync(Arg.Is<BodyMetric>(m => m.UserId == TestUserId), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    // --- Update ---

    [Fact]
    public async Task Update_ShouldSucceed_WhenOwner()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new UpdateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new UpdateBodyMetricCommand(metric.Id, new DateTime(2025, 7, 1), 78m, 180m, null, null, null, null, null, null, null, null, null, "Updated"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        metric.Weight!.Value.Should().Be(78m);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenNotExists()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((BodyMetric?)null);
        var sut = new UpdateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new UpdateBodyMetricCommand(Guid.NewGuid(), DateTime.Now, 80m, null, null, null, null, null, null, null, null, null, null, null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("BodyMetric.NotFound");
    }

    [Fact]
    public async Task Update_ShouldReturnForbidden_WhenNotOwner()
    {
        var otherUserId = Guid.NewGuid();
        var metric = BodyMetric.Create(otherUserId, new DateTime(2025, 6, 1), 80m, 180m, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new UpdateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new UpdateBodyMetricCommand(metric.Id, DateTime.Now, 78m, null, null, null, null, null, null, null, null, null, null, null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Error.Forbidden");
    }

    [Fact]
    public async Task Update_ShouldAllowAdmin_WhenNotOwner()
    {
        var otherUserId = Guid.NewGuid();
        var metric = BodyMetric.Create(otherUserId, new DateTime(2025, 6, 1), 80m, 180m, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        _currentUser.IsAdmin.Returns(true);
        var sut = new UpdateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new UpdateBodyMetricCommand(metric.Id, DateTime.Now, 78m, null, null, null, null, null, null, null, null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    // --- Delete ---

    [Fact]
    public async Task Delete_ShouldSucceed_WhenOwner()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new DeleteBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new DeleteBodyMetricCommand(metric.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        metric.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task Delete_ShouldReturnForbidden_WhenNotOwner()
    {
        var metric = BodyMetric.Create(Guid.NewGuid(), new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new DeleteBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new DeleteBodyMetricCommand(metric.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Error.Forbidden");
    }

    // --- Activate ---

    [Fact]
    public async Task Activate_ShouldSucceed_WhenOwnerAndDeleted()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        metric.Delete();
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new ActivateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new ActivateBodyMetricCommand(metric.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        metric.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Activate_ShouldReturnAlreadyActive_WhenActive()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        metric.Activate();
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new ActivateBodyMetricCommandHandler(_repository, _unitOfWork, _currentUser);

        var result = await sut.Handle(new ActivateBodyMetricCommand(metric.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("BodyMetric.AlreadyActive");
    }

    // --- GetById ---

    [Fact]
    public async Task GetById_ShouldReturnDto_WhenOwner()
    {
        var metric = BodyMetric.Create(TestUserId, new DateTime(2025, 6, 1), 80m, 180m, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new GetBodyMetricByIdQueryHandler(_repository, _currentUser);

        var result = await sut.Handle(new GetBodyMetricByIdQuery(metric.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Weight.Should().Be(80m);
    }

    [Fact]
    public async Task GetById_ShouldReturnForbidden_WhenNotOwner()
    {
        var metric = BodyMetric.Create(Guid.NewGuid(), new DateTime(2025, 6, 1), 80m, null, null, null, null, null, null, null, null, null, null, null);
        _repository.GetByIdAsync(metric.Id, Arg.Any<CancellationToken>()).Returns(metric);
        var sut = new GetBodyMetricByIdQueryHandler(_repository, _currentUser);

        var result = await sut.Handle(new GetBodyMetricByIdQuery(metric.Id), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Error.Forbidden");
    }
}



