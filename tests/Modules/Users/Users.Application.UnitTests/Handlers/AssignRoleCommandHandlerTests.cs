using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Users.AssignRole;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class AssignRoleCommandHandlerTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly AssignRoleCommandHandler _sut;

    public AssignRoleCommandHandlerTests()
    {
        _sut = new AssignRoleCommandHandler(_userRepository, _roleRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldAssignRole_WhenUserAndRoleExist()
    {
        var user = User.Create("test@example.com", "$2a$12$hashed", "John", "Doe");
        var roleId = Guid.NewGuid();
        var command = new AssignRoleCommand(user.Id, roleId);
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _roleRepository.ExistsAsync(roleId, Arg.Any<CancellationToken>()).Returns(true);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        var command = new AssignRoleCommand(Guid.NewGuid(), Guid.NewGuid());
        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns((User?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("User.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRoleNotFound()
    {
        var user = User.Create("test@example.com", "$2a$12$hashed", "John", "Doe");
        var command = new AssignRoleCommand(user.Id, Guid.NewGuid());
        _userRepository.GetByIdAsync(user.Id, Arg.Any<CancellationToken>()).Returns(user);
        _roleRepository.ExistsAsync(command.RoleId, Arg.Any<CancellationToken>()).Returns(false);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Role.NotFound");
    }
}
