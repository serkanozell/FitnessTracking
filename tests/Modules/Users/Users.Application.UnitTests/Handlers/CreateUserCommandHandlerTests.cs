using BuildingBlocks.Domain.Security;
using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Users.CreateUser;
using Users.Domain.Constants;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class CreateUserCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly IPasswordHasher _passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly CreateUserCommandHandler _sut;

    public CreateUserCommandHandlerTests()
    {
        _passwordHasher.Hash(Arg.Any<string>()).Returns("hashed");
        _sut = new CreateUserCommandHandler(_userRepository, _roleRepository, _unitOfWork, _passwordHasher);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WithSpecifiedRoles()
    {
        var adminRoleId = RoleConstants.AdminRoleId;
        var command = new CreateUserCommand("admin@test.com", "Password123!", "Admin", "User", [adminRoleId]);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
        _roleRepository.ExistsAsync(adminRoleId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.Email == "admin@test.com" &&
            u.UserRoles.Any(ur => ur.RoleId == adminRoleId)),
            Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldAssignMemberRole_WhenNoRolesProvided()
    {
        var command = new CreateUserCommand("user@test.com", "Password123!", "Test", "User", []);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
        _roleRepository.ExistsAsync(RoleConstants.MemberRoleId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.UserRoles.Any(ur => ur.RoleId == RoleConstants.MemberRoleId)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldAssignMultipleRoles()
    {
        var command = new CreateUserCommand("multi@test.com", "Password123!", "Multi", "Role",
            [RoleConstants.AdminRoleId, RoleConstants.MemberRoleId]);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
        _roleRepository.ExistsAsync(RoleConstants.AdminRoleId, Arg.Any<CancellationToken>()).Returns(true);
        _roleRepository.ExistsAsync(RoleConstants.MemberRoleId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _userRepository.Received(1).AddAsync(Arg.Is<User>(u =>
            u.UserRoles.Count == 2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenEmailAlreadyExists()
    {
        var command = new CreateUserCommand("existing@test.com", "Password123!", "Test", "User", []);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.DuplicateEmail");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRoleDoesNotExist()
    {
        var invalidRoleId = Guid.NewGuid();
        var command = new CreateUserCommand("user@test.com", "Password123!", "Test", "User", [invalidRoleId]);
        _userRepository.ExistsByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(false);
        _roleRepository.ExistsAsync(invalidRoleId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Role.NotFound");
    }
}