using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Security;
using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Users.Login;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class LoginCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenService _tokenService = Substitute.For<ITokenService>();
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        _tokenService.RefreshTokenExpirationDays.Returns(7);
        _refreshTokenRepository.GetActiveByUserIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(new List<RefreshToken>());
        _sut = new LoginCommandHandler(_userRepository, _refreshTokenRepository, _unitOfWork, _passwordHasher, _tokenService);
    }

    [Fact]
    public async Task Handle_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = User.Create("test@example.com", "$2a$12$hashed", "John", "Doe");
        var command = new LoginCommand("test@example.com", "Password123");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(true);
        _tokenService.GenerateToken(user.Id, user.Email, Arg.Any<IEnumerable<string>>()).Returns("jwt-token");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().Be("jwt-token");
        result.Data.RefreshToken.Should().NotBeNullOrEmpty();
        result.Data.UserId.Should().Be(user.Id);
        result.Data.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidCredentials_WhenUserNotFound()
    {
        var command = new LoginCommand("notfound@example.com", "Password123");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidCredentials");
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidCredentials_WhenPasswordIsWrong()
    {
        var user = User.Create("test@example.com", "$2a$12$hashed", "John", "Doe");
        var command = new LoginCommand("test@example.com", "WrongPassword");
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.Password, user.PasswordHash).Returns(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidCredentials");
    }

    [Fact]
    public async Task Handle_ShouldRevokeExistingRefreshTokens_WhenLoginSucceeds()
    {
        var user = User.Create("test@example.com", "$2a$12$hashed", "John", "Doe");
        var existingToken = RefreshToken.Create(user.Id);
        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify("Password123", user.PasswordHash).Returns(true);
        _tokenService.GenerateToken(user.Id, user.Email, Arg.Any<IEnumerable<string>>()).Returns("jwt-token");
        _refreshTokenRepository.GetActiveByUserIdAsync(user.Id, Arg.Any<CancellationToken>())
            .Returns(new List<RefreshToken> { existingToken });

        await _sut.Handle(new LoginCommand("test@example.com", "Password123"), CancellationToken.None);

        existingToken.IsRevoked.Should().BeTrue();
        _refreshTokenRepository.Received(1).Update(existingToken);
    }
}
