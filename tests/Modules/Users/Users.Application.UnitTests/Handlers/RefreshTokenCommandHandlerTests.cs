using BuildingBlocks.Application.Abstractions;
using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Users.RefreshToken;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class RefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly RefreshTokenCommandHandler _sut;

    public RefreshTokenCommandHandlerTests()
    {
        _tokenService.RefreshTokenExpirationDays.Returns(7);
        _tokenService.GenerateToken(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<IEnumerable<string>>())
            .Returns("new-jwt-token");
        _sut = new RefreshTokenCommandHandler(_refreshTokenRepository, _userRepository, _unitOfWork, _tokenService);
    }

    [Fact]
    public async Task Handle_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        var user = User.Create("test@example.com", "hash", "John", "Doe");
        var existingToken = Domain.Entity.RefreshToken.Create(user.Id);
        _refreshTokenRepository.GetByTokenAsync(existingToken.Token, Arg.Any<CancellationToken>()).Returns(existingToken);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        var command = new RefreshTokenCommand(existingToken.Token);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().Be("new-jwt-token");
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        result.Data.RefreshToken.Should().NotBe(existingToken.Token);
        result.Data.UserId.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_ShouldRevokeExistingToken_WhenRefreshSucceeds()
    {
        var user = User.Create("test@example.com", "hash", "John", "Doe");
        var existingToken = Domain.Entity.RefreshToken.Create(user.Id);
        _refreshTokenRepository.GetByTokenAsync(existingToken.Token, Arg.Any<CancellationToken>()).Returns(existingToken);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        await _sut.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        existingToken.IsRevoked.Should().BeTrue();
        _refreshTokenRepository.Received(1).Update(existingToken);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTokenNotFound()
    {
        _refreshTokenRepository.GetByTokenAsync("nonexistent", Arg.Any<CancellationToken>())
            .Returns((Domain.Entity.RefreshToken?)null);

        var result = await _sut.Handle(new RefreshTokenCommand("nonexistent"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidRefreshToken");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTokenIsRevoked()
    {
        var existingToken = Domain.Entity.RefreshToken.Create(Guid.NewGuid());
        existingToken.Revoke();
        _refreshTokenRepository.GetByTokenAsync(existingToken.Token, Arg.Any<CancellationToken>()).Returns(existingToken);

        var result = await _sut.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidRefreshToken");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var existingToken = Domain.Entity.RefreshToken.Create(Guid.NewGuid());
        _refreshTokenRepository.GetByTokenAsync(existingToken.Token, Arg.Any<CancellationToken>()).Returns(existingToken);
        _userRepository.GetByIdAsync(existingToken.UserId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidRefreshToken");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserIsInactive()
    {
        var user = User.Create("test@example.com", "hash", "John", "Doe");
        user.Delete("test-admin");
        var existingToken = Domain.Entity.RefreshToken.Create(user.Id);
        _refreshTokenRepository.GetByTokenAsync(existingToken.Token, Arg.Any<CancellationToken>()).Returns(existingToken);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);

        var result = await _sut.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidRefreshToken");
    }
}
