using BuildingBlocks.Domain.Exceptions;
using FluentAssertions;
using Users.Domain.Entity;
using Users.Domain.Enums;
using Users.Domain.Events;
using Xunit;

namespace Users.Domain.UnitTests;

public class UserTests
{
    private const string DefaultEmail = "test@example.com";
    private const string DefaultPasswordHash = "$2a$12$hashedpassword";
    private const string DefaultFirstName = "John";
    private const string DefaultLastName = "Doe";

    private static User CreateDefaultUser()
        => User.Create(DefaultEmail, DefaultPasswordHash, DefaultFirstName, DefaultLastName);

    // --- Create ---

    [Fact]
    public void Create_ShouldSetProperties()
    {
        var user = User.Create(DefaultEmail, DefaultPasswordHash, DefaultFirstName, DefaultLastName);

        user.Id.Should().NotBeEmpty();
        user.Email.Should().Be(DefaultEmail);
        user.PasswordHash.Should().Be(DefaultPasswordHash);
        user.FirstName.Should().Be(DefaultFirstName);
        user.LastName.Should().Be(DefaultLastName);
        user.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public void Create_ShouldRaiseUserRegisteredEvent()
    {
        var user = User.Create(DefaultEmail, DefaultPasswordHash, DefaultFirstName, DefaultLastName);

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserRegisteredEvent>()
            .Which.Should().Match<UserRegisteredEvent>(e =>
                e.UserId == user.Id && e.Email == DefaultEmail);
    }

    // --- UpdateProfile ---

    [Fact]
    public void UpdateProfile_ShouldChangeFirstNameAndLastName()
    {
        var user = CreateDefaultUser();
        user.ClearDomainEvents();

        user.UpdateProfile("Jane", "Smith");

        user.FirstName.Should().Be("Jane");
        user.LastName.Should().Be("Smith");
    }

    [Fact]
    public void UpdateProfile_ShouldRaiseUserProfileUpdatedEvent()
    {
        var user = CreateDefaultUser();
        user.ClearDomainEvents();

        user.UpdateProfile("Jane", "Smith");

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserProfileUpdatedEvent>()
            .Which.UserId.Should().Be(user.Id);
    }

    // --- ChangePassword ---

    [Fact]
    public void ChangePassword_ShouldUpdatePasswordHash()
    {
        var user = CreateDefaultUser();
        var newHash = "$2a$12$newhashedpassword";
        user.ClearDomainEvents();

        user.ChangePassword(newHash);

        user.PasswordHash.Should().Be(newHash);
    }

    [Fact]
    public void ChangePassword_ShouldRaiseUserPasswordChangedEvent()
    {
        var user = CreateDefaultUser();
        user.ClearDomainEvents();

        user.ChangePassword("$2a$12$newhashedpassword");

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserPasswordChangedEvent>()
            .Which.UserId.Should().Be(user.Id);
    }

    // --- AssignRole ---

    [Fact]
    public void AssignRole_ShouldAddUserRole()
    {
        var user = CreateDefaultUser();
        var roleId = Guid.NewGuid();
        user.ClearDomainEvents();

        var userRole = user.AssignRole(roleId);

        userRole.Should().NotBeNull();
        userRole.RoleId.Should().Be(roleId);
        userRole.UserId.Should().Be(user.Id);
        user.UserRoles.Should().ContainSingle();
    }

    [Fact]
    public void AssignRole_ShouldRaiseUserRoleAssignedEvent()
    {
        var user = CreateDefaultUser();
        var roleId = Guid.NewGuid();
        user.ClearDomainEvents();

        user.AssignRole(roleId);

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserRoleAssignedEvent>()
            .Which.Should().Match<UserRoleAssignedEvent>(e =>
                e.UserId == user.Id && e.RoleId == roleId);
    }

    [Fact]
    public void AssignRole_ShouldThrowBusinessRuleViolation_WhenRoleAlreadyAssigned()
    {
        var user = CreateDefaultUser();
        var roleId = Guid.NewGuid();
        user.AssignRole(roleId);

        var act = () => user.AssignRole(roleId);

        act.Should().Throw<BusinessRuleViolationException>();
    }

    // --- RemoveRole ---

    [Fact]
    public void RemoveRole_ShouldRemoveUserRole()
    {
        var user = CreateDefaultUser();
        var roleId = Guid.NewGuid();
        user.AssignRole(roleId);
        user.ClearDomainEvents();

        user.RemoveRole(roleId);

        user.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void RemoveRole_ShouldRaiseUserRoleRemovedEvent()
    {
        var user = CreateDefaultUser();
        var roleId = Guid.NewGuid();
        user.AssignRole(roleId);
        user.ClearDomainEvents();

        user.RemoveRole(roleId);

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserRoleRemovedEvent>()
            .Which.Should().Match<UserRoleRemovedEvent>(e =>
                e.UserId == user.Id && e.RoleId == roleId);
    }

    [Fact]
    public void RemoveRole_ShouldThrowDomainNotFoundException_WhenRoleNotAssigned()
    {
        var user = CreateDefaultUser();

        var act = () => user.RemoveRole(Guid.NewGuid());

        act.Should().Throw<DomainNotFoundException>();
    }

    // --- Activate ---

    [Fact]
    public void Activate_ShouldSetIsActiveTrueAndIsDeletedFalse()
    {
        var user = CreateDefaultUser();
        user.Delete();
        user.ClearDomainEvents();

        user.Activate();

        user.IsActive.Should().BeTrue();
        user.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldRaiseUserActivatedEvent()
    {
        var user = CreateDefaultUser();
        user.ClearDomainEvents();

        user.Activate();

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserActivatedEvent>()
            .Which.UserId.Should().Be(user.Id);
    }

    // --- Delete ---

    [Fact]
    public void Delete_ShouldSetIsActiveFalseAndIsDeletedTrue()
    {
        var user = CreateDefaultUser();
        user.ClearDomainEvents();

        user.Delete();

        user.IsActive.Should().BeFalse();
        user.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_ShouldRaiseUserDeletedEvent()
    {
        var user = CreateDefaultUser();
        user.ClearDomainEvents();

        user.Delete();

        user.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<UserDeletedEvent>()
            .Which.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Delete_ShouldSoftDeleteUserRoles()
    {
        var user = CreateDefaultUser();
        var roleId = Guid.NewGuid();
        user.AssignRole(roleId);
        user.ClearDomainEvents();

        user.Delete();

        user.UserRoles.Should().AllSatisfy(ur =>
        {
            ur.IsDeleted.Should().BeTrue();
            ur.IsActive.Should().BeFalse();
        });
    }
}