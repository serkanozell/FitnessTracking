using BuildingBlocks.Domain.Security;
using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Users.ChangePassword;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class ChangePasswordCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ChangePasswordCommandHandler _sut;

    public ChangePasswordCommandHandlerTests()
    {
        _sut = new ChangePasswordCommandHandler(_userRepository, _unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task Handle_ShouldChangePassword_WhenCurrentPasswordIsCorrect()
    {
        var user = User.Create("test@example.com", "$2a$12$oldhash", "John", "Doe");
        var command = new ChangePasswordCommand(user.Id, "OldPassword", "NewPassword123");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.CurrentPassword, user.PasswordHash).Returns(true);
        _passwordHasher.Hash(command.NewPassword).Returns("$2a$12$newhash");

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _userRepository.Received(1).Update(user);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "Old", "New123456");
        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidCredentials_WhenCurrentPasswordIsWrong()
    {
        var user = User.Create("test@example.com", "$2a$12$oldhash", "John", "Doe");
        var command = new ChangePasswordCommand(user.Id, "WrongPassword", "NewPassword123");
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.Verify(command.CurrentPassword, user.PasswordHash).Returns(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.InvalidCredentials");
    }
}
