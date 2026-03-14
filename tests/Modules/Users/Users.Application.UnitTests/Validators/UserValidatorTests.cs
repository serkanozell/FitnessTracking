using FluentValidation.TestHelper;
using Users.Application.Features.Users.Register;
using Users.Application.Features.Users.Login;
using Users.Application.Features.Users.ChangePassword;
using Users.Application.Features.Users.UpdateProfile;
using Xunit;

namespace Users.Application.UnitTests.Validators;

public class RegisterCommandValidatorTests
{
    private readonly RegisterCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new RegisterCommand("test@example.com", "Password123", "John", "Doe");
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void ShouldFail_WhenEmailIsInvalid(string? email)
    {
        var command = new RegisterCommand(email!, "Password123", "John", "Doe");
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    public void ShouldFail_WhenPasswordIsInvalid(string? password)
    {
        var command = new RegisterCommand("test@example.com", password!, "John", "Doe");
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldFail_WhenFirstNameIsEmpty(string? firstName)
    {
        var command = new RegisterCommand("test@example.com", "Password123", firstName!, "Doe");
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldFail_WhenLastNameIsEmpty(string? lastName)
    {
        var command = new RegisterCommand("test@example.com", "Password123", "John", lastName!);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
    }
}

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new LoginCommand("test@example.com", "Password123");
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldFail_WhenEmailIsEmpty(string? email)
    {
        var command = new LoginCommand(email!, "Password123");
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ShouldFail_WhenPasswordIsEmpty(string? password)
    {
        var command = new LoginCommand("test@example.com", password!);
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}

public class ChangePasswordCommandValidatorTests
{
    private readonly ChangePasswordCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "OldPassword", "NewPassword123");
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenNewPasswordIsTooShort()
    {
        var command = new ChangePasswordCommand(Guid.NewGuid(), "OldPassword", "12345");
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }
}

public class UpdateProfileCommandValidatorTests
{
    private readonly UpdateProfileCommandValidator _sut = new();

    [Fact]
    public void ShouldPass_WhenCommandIsValid()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "Jane", "Smith");
        var result = _sut.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ShouldFail_WhenFirstNameExceedsMaxLength()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), new string('A', 101), "Smith");
        var result = _sut.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
    }
}
