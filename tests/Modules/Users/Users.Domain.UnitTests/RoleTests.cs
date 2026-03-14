using FluentAssertions;
using Users.Domain.Entity;
using Users.Domain.Events;
using Xunit;

namespace Users.Domain.UnitTests;

public class RoleTests
{
    private const string DefaultName = "Admin";
    private const string DefaultDescription = "Administrator role";

    private static Role CreateDefaultRole()
        => Role.Create(DefaultName, DefaultDescription);

    // --- Create ---

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var role = Role.Create(DefaultName, DefaultDescription);

        role.Id.Should().NotBeEmpty();
        role.Name.Should().Be(DefaultName);
        role.Description.Should().Be(DefaultDescription);
    }

    [Fact]
    public void Create_ShouldRaiseRoleCreatedEvent()
    {
        var role = Role.Create(DefaultName, DefaultDescription);

        role.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RoleCreatedEvent>()
            .Which.Should().Match<RoleCreatedEvent>(e =>
                e.RoleId == role.Id && e.Name == DefaultName);
    }

    [Fact]
    public void Create_WithNullDescription_ShouldSetNullDescription()
    {
        var role = Role.Create(DefaultName, null);

        role.Description.Should().BeNull();
    }

    // --- Update ---

    [Fact]
    public void Update_ShouldChangeProperties()
    {
        var role = CreateDefaultRole();
        role.ClearDomainEvents();

        role.Update("SuperAdmin", "Super administrator role");

        role.Name.Should().Be("SuperAdmin");
        role.Description.Should().Be("Super administrator role");
    }

    [Fact]
    public void Update_ShouldRaiseRoleUpdatedEvent()
    {
        var role = CreateDefaultRole();
        role.ClearDomainEvents();

        role.Update("SuperAdmin", "Super administrator role");

        role.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RoleUpdatedEvent>()
            .Which.RoleId.Should().Be(role.Id);
    }

    // --- Activate ---

    [Fact]
    public void Activate_ShouldSetIsActiveTrueAndIsDeletedFalse()
    {
        var role = CreateDefaultRole();
        role.Delete();
        role.ClearDomainEvents();

        role.Activate();

        role.IsActive.Should().BeTrue();
        role.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldRaiseRoleActivatedEvent()
    {
        var role = CreateDefaultRole();
        role.ClearDomainEvents();

        role.Activate();

        role.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RoleActivatedEvent>()
            .Which.RoleId.Should().Be(role.Id);
    }

    // --- Delete ---

    [Fact]
    public void Delete_ShouldSetIsActiveFalseAndIsDeletedTrue()
    {
        var role = CreateDefaultRole();
        role.ClearDomainEvents();

        role.Delete();

        role.IsActive.Should().BeFalse();
        role.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_ShouldRaiseRoleDeletedEvent()
    {
        var role = CreateDefaultRole();
        role.ClearDomainEvents();

        role.Delete();

        role.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<RoleDeletedEvent>()
            .Which.RoleId.Should().Be(role.Id);
    }
}
