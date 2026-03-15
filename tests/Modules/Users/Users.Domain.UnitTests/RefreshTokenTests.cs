using FluentAssertions;
using Users.Domain.Entity;
using Xunit;

namespace Users.Domain.UnitTests;

public class RefreshTokenTests
{
    // --- Create ---

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var userId = Guid.NewGuid();

        var token = RefreshToken.Create(userId);

        token.Id.Should().NotBeEmpty();
        token.UserId.Should().Be(userId);
        token.Token.Should().NotBeNullOrEmpty();
        token.CreatedOnUtc.ToLocalTime().Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
        token.ExpiresOnUtc.ToLocalTime().Should().BeCloseTo(DateTime.Now.AddDays(7), TimeSpan.FromSeconds(5));
        token.RevokedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldUseCustomExpirationDays()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), expirationDays: 30);

        token.ExpiresOnUtc.ToLocalTime().Should().BeCloseTo(DateTime.Now.AddDays(30), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_ShouldGenerateUniqueTokens()
    {
        var token1 = RefreshToken.Create(Guid.NewGuid());
        var token2 = RefreshToken.Create(Guid.NewGuid());

        token1.Token.Should().NotBe(token2.Token);
    }

    // --- IsActive ---

    [Fact]
    public void IsActive_ShouldReturnTrue_WhenNotExpiredAndNotRevoked()
    {
        var token = RefreshToken.Create(Guid.NewGuid());

        token.IsActive.Should().BeTrue();
        token.IsExpired.Should().BeFalse();
        token.IsRevoked.Should().BeFalse();
    }

    // --- Revoke ---

    [Fact]
    public void Revoke_ShouldSetRevokedOnUtc()
    {
        var token = RefreshToken.Create(Guid.NewGuid());

        token.Revoke();

        token.RevokedOnUtc.Should().NotBeNull();
        token.RevokedOnUtc.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Revoke_ShouldMakeTokenInactive()
    {
        var token = RefreshToken.Create(Guid.NewGuid());

        token.Revoke();

        token.IsActive.Should().BeFalse();
        token.IsRevoked.Should().BeTrue();
    }
}
