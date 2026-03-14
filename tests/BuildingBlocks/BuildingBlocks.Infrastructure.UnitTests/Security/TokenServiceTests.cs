using BuildingBlocks.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Security;

public class TokenServiceTests
{
    private static readonly JwtOptions ValidOptions = new()
    {
        Key = "ThisIsASecretKeyForTestingPurposes_MustBe32Chars!",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpirationMinutes = 60
    };

    private readonly TokenService _sut = new(Options.Create(ValidOptions));

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtString()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "test@test.com", ["Admin"]);

        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GenerateToken_ShouldContainSubjectClaim()
    {
        var userId = Guid.NewGuid();

        var token = _sut.GenerateToken(userId, "test@test.com", []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
    }

    [Fact]
    public void GenerateToken_ShouldContainEmailClaim()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "user@example.com", []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "user@example.com");
    }

    [Fact]
    public void GenerateToken_ShouldContainRoleClaims()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "test@test.com", ["Admin", "User"]);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        var roles = jwt.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        roles.Should().Contain("Admin");
        roles.Should().Contain("User");
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectIssuer()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "test@test.com", []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Issuer.Should().Be("TestIssuer");
    }

    [Fact]
    public void GenerateToken_ShouldSetCorrectAudience()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "test@test.com", []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Audiences.Should().Contain("TestAudience");
    }

    [Fact]
    public void GenerateToken_ShouldSetExpiration()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "test@test.com", []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.ValidTo.Should().BeAfter(DateTime.Now);
    }

    [Fact]
    public void GenerateToken_ShouldHandleNullEmail()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), null, []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == string.Empty);
    }

    [Fact]
    public void GenerateToken_ShouldHandleEmptyRoles()
    {
        var token = _sut.GenerateToken(Guid.NewGuid(), "test@test.com", []);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        jwt.Claims.Where(c => c.Type == "role" || c.Type == ClaimTypes.Role).Should().BeEmpty();
    }

    [Fact]
    public void GenerateToken_ShouldThrow_WhenKeyIsMissing()
    {
        var sut = new TokenService(Options.Create(new JwtOptions { Key = "" }));

        var act = () => sut.GenerateToken(Guid.NewGuid(), "test@test.com", []);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenOptionsIsNull()
    {
        var act = () => new TokenService(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
