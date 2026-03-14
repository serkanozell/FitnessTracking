using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Roles.UpdateRole;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class UpdateRoleCommandHandlerTests
{
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly UpdateRoleCommandHandler _sut;

    public UpdateRoleCommandHandlerTests()
    {
        _sut = new UpdateRoleCommandHandler(_roleRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldUpdateRole_WhenValid()
    {
        var role = Role.Create("Admin", "Old description");
        var command = new UpdateRoleCommand(role.Id, "SuperAdmin", "New description");
        _roleRepository.GetByIdAsync(role.Id, Arg.Any<CancellationToken>()).Returns(role);
        _roleRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((Role?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _roleRepository.Received(1).Update(role);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenRoleNotFound()
    {
        var command = new UpdateRoleCommand(Guid.NewGuid(), "Admin", null);
        _roleRepository.GetByIdAsync(command.RoleId, Arg.Any<CancellationToken>()).Returns((Role?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Role.NotFound");
    }

    [Fact]
    public async Task Handle_ShouldReturnDuplicateNameError_WhenNameTakenByAnotherRole()
    {
        var role = Role.Create("Admin", "Description");
        var otherRole = Role.Create("SuperAdmin", "Other");
        var command = new UpdateRoleCommand(role.Id, "SuperAdmin", "New desc");
        _roleRepository.GetByIdAsync(role.Id, Arg.Any<CancellationToken>()).Returns(role);
        _roleRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns(otherRole);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Role.DuplicateName");
    }

    [Fact]
    public async Task Handle_ShouldAllowSameName_WhenUpdatingSameRole()
    {
        var role = Role.Create("Admin", "Old description");
        var command = new UpdateRoleCommand(role.Id, "Admin", "New description");
        _roleRepository.GetByIdAsync(role.Id, Arg.Any<CancellationToken>()).Returns(role);
        _roleRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns(role);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }
}
