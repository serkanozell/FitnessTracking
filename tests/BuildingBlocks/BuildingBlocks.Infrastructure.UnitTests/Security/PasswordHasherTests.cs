using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Infrastructure.UnitTests.Security;

public class PasswordHasherTests
{
    private readonly PasswordHasher _sut = new(workFactor: 4);

    [Fact]
    public void Hash_ShouldReturnBCryptHash()
    {
        var hash = _sut.Hash("MyPassword123!");

        hash.Should().NotBeNullOrEmpty();
        hash.Should().StartWith("$2");
    }

    [Fact]
    public void Hash_ShouldReturnDifferentHashes_ForSamePassword()
    {
        var hash1 = _sut.Hash("SamePassword");
        var hash2 = _sut.Hash("SamePassword");

        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void Hash_ShouldThrowArgumentException_WhenPasswordIsEmpty()
    {
        var act = () => _sut.Hash("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Hash_ShouldThrowArgumentException_WhenPasswordIsNull()
    {
        var act = () => _sut.Hash(null!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Hash_ShouldThrowArgumentException_WhenPasswordIsWhitespace()
    {
        var act = () => _sut.Hash("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Verify_ShouldReturnTrue_WhenPasswordMatchesHash()
    {
        var password = "Correct_Password!";
        var hash = _sut.Hash(password);

        var result = _sut.Verify(password, hash);

        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenPasswordDoesNotMatch()
    {
        var hash = _sut.Hash("OriginalPassword");

        var result = _sut.Verify("WrongPassword", hash);

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenPasswordIsEmpty()
    {
        var result = _sut.Verify("", "$2a$04$somehash");

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenHashIsEmpty()
    {
        var result = _sut.Verify("password", "");

        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_ShouldReturnFalse_WhenBothAreNull()
    {
        var result = _sut.Verify(null!, null!);

        result.Should().BeFalse();
    }
}
