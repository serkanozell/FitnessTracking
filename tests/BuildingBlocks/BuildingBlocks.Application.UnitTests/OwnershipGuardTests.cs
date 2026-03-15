using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace BuildingBlocks.Application.UnitTests;

public class OwnershipGuardTests
{
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();

    [Fact]
    public void CheckOwnership_ShouldReturnNull_WhenUserOwnsResource()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId.ToString());
        _currentUser.IsAdmin.Returns(false);

        var result = OwnershipGuard.CheckOwnership(_currentUser, userId);

        result.Should().BeNull();
    }

    [Fact]
    public void CheckOwnership_ShouldReturnForbidden_WhenUserDoesNotOwnResource()
    {
        _currentUser.UserId.Returns(Guid.NewGuid().ToString());
        _currentUser.IsAdmin.Returns(false);

        var result = OwnershipGuard.CheckOwnership(_currentUser, Guid.NewGuid());

        result.Should().NotBeNull();
        result!.Code.Should().Be("Error.Forbidden");
    }

    [Fact]
    public void CheckOwnership_ShouldReturnNull_WhenUserIsAdmin()
    {
        _currentUser.UserId.Returns(Guid.NewGuid().ToString());
        _currentUser.IsAdmin.Returns(true);

        var result = OwnershipGuard.CheckOwnership(_currentUser, Guid.NewGuid());

        result.Should().BeNull();
    }
}
