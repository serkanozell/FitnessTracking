using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Users.Domain.Entity;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Repositories;
using Xunit;

namespace Users.Infrastructure.IntegrationTests;

[Collection("SqlServer")]
public class UserRepositoryTests : IAsyncLifetime
{
    private readonly UsersDbContext _context;
    private readonly UserRepository _sut;

    public UserRepositoryTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("UserRepoTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new UsersDbContext(options);
        _sut = new UserRepository(_context);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        var user = User.Create("test@example.com", "$2a$12$hashed", "John", "Doe");

        await _sut.AddAsync(user);
        await _context.SaveChangesAsync();

        var saved = await _context.Users.FindAsync(user.Id);
        saved.Should().NotBeNull();
        saved!.Email.Should().Be("test@example.com");
        saved.FirstName.Should().Be("John");
        saved.LastName.Should().Be("Doe");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        var user = User.Create("find@example.com", "$2a$12$hashed", "Jane", "Smith");
        await _sut.AddAsync(user);
        await _context.SaveChangesAsync();

        var found = await _sut.GetByEmailAsync("find@example.com");

        found.Should().NotBeNull();
        found!.Email.Should().Be("find@example.com");
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotExists()
    {
        var found = await _sut.GetByEmailAsync("notfound@example.com");

        found.Should().BeNull();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrue_WhenExists()
    {
        var user = User.Create("exists@example.com", "$2a$12$hashed", "John", "Doe");
        await _sut.AddAsync(user);
        await _context.SaveChangesAsync();

        var exists = await _sut.ExistsByEmailAsync("exists@example.com");

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnFalse_WhenNotExists()
    {
        var exists = await _sut.ExistsByEmailAsync("nope@example.com");

        exists.Should().BeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeUserRoles()
    {
        var role = Role.Create("TestRole", "Test");
        await _context.Roles.AddAsync(role);
        var user = User.Create("withrole@example.com", "$2a$12$hashed", "John", "Doe");
        user.AssignRole(role.Id);
        await _sut.AddAsync(user);
        await _context.SaveChangesAsync();

        var found = await _sut.GetByIdAsync(user.Id);

        found.Should().NotBeNull();
        found!.UserRoles.Should().ContainSingle();
        found.UserRoles[0].Role.Name.Should().Be("TestRole");
    }
}
