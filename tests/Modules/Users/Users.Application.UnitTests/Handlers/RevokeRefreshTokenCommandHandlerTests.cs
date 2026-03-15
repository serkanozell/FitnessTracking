using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Users.RevokeRefreshToken;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class RevokeRefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly RevokeRefreshTokenCommandHandler _sut;

    public RevokeRefreshTokenCommandHandlerTests()
    {
        _sut = new RevokeRefreshTokenCommandHandler(_refreshTokenRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldRevokeToken_WhenTokenIsValid()
    {
        var token = RefreshToken.Create(Guid.NewGuid());
        _refreshTokenRepository.GetByTokenAsync(token.Token, Arg.Any<CancellationToken>()).Returns(token);

        var result = await _sut.Handle(new RevokeRefreshTokenCommand(token.Token), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        token.IsRevoked.Should().BeTrue();
        _refreshTokenRepository.Received(1).Update(token);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTokenNotFound()
    {
        _refreshTokenRepository.GetByTokenAsync("invalid", Arg.Any<CancellationToken>())
            .Returns((RefreshToken?)null);

        var result = await _sut.Handle(new RevokeRefreshTokenCommand("invalid"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidRefreshToken");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenTokenAlreadyRevoked()
    {
        var token = RefreshToken.Create(Guid.NewGuid());
        token.Revoke();
        _refreshTokenRepository.GetByTokenAsync(token.Token, Arg.Any<CancellationToken>()).Returns(token);

        var result = await _sut.Handle(new RevokeRefreshTokenCommand(token.Token), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidRefreshToken");
    }
}
