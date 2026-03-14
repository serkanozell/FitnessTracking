using FluentAssertions;
using NSubstitute;
using Users.Application.Features.Roles.CreateRole;
using Users.Domain.Entity;
using Users.Domain.Repositories;
using Xunit;

namespace Users.Application.UnitTests.Handlers;

public class CreateRoleCommandHandlerTests
{
    private readonly IRoleRepository _roleRepository = Substitute.For<IRoleRepository>();
    private readonly IUsersUnitOfWork _unitOfWork = Substitute.For<IUsersUnitOfWork>();
    private readonly CreateRoleCommandHandler _sut;

    public CreateRoleCommandHandlerTests()
    {
        _sut = new CreateRoleCommandHandler(_roleRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateRole_WhenNameIsUnique()
    {
        var command = new CreateRoleCommand("Admin", "Administrator role");
        _roleRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns((Role?)null);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeEmpty();
        await _roleRepository.Received(1).AddAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnDuplicateNameError_WhenNameExists()
    {
        var existingRole = Role.Create("Admin", "Existing");
        var command = new CreateRoleCommand("Admin", "New description");
        _roleRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>()).Returns(existingRole);

        var result = await _sut.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Role.DuplicateName");
        await _roleRepository.DidNotReceive().AddAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());
    }
}
