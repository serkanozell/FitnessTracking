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
public class RoleRepositoryTests : IAsyncLifetime
{
    private readonly UsersDbContext _context;
    private readonly RoleRepository _sut;

    public RoleRepositoryTests(SqlServerContainerFixture fixture)
    {
        var currentUser = Substitute.For<ICurrentUser>();
        currentUser.IsAuthenticated.Returns(true);
        currentUser.UserId.Returns("test-user");

        var options = new DbContextOptionsBuilder<UsersDbContext>()
            .UseSqlServer(fixture.GetDatabaseConnectionString("RoleRepoTests"))
            .AddInterceptors(new AuditableEntityInterceptor(currentUser))
            .Options;

        _context = new UsersDbContext(options);
        _sut = new RoleRepository(_context);
    }

    public async ValueTask InitializeAsync() => await _context.Database.EnsureCreatedAsync();
    public async ValueTask DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldPersistRole()
    {
        var role = Role.Create("Test", "Test role");

        await _sut.AddAsync(role);
        await _context.SaveChangesAsync();

        var saved = await _context.Roles.FindAsync(role.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Test");
        saved.Description.Should().Be("Test role");
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnRole_WhenExists()
    {
        var role = Role.Create("Moderator", "Moderator role");
        await _sut.AddAsync(role);
        await _context.SaveChangesAsync();

        var found = await _sut.GetByNameAsync("Moderator");

        found.Should().NotBeNull();
        found!.Name.Should().Be("Moderator");
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnNull_WhenNotExists()
    {
        var found = await _sut.GetByNameAsync("NonExistent");

        found.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRoles()
    {
        await _sut.AddAsync(Role.Create("Role1", "Desc1"));
        await _sut.AddAsync(Role.Create("Role2", "Desc2"));
        await _context.SaveChangesAsync();

        var roles = await _sut.GetAllAsync();

        roles.Should().HaveCount(4); // 2 seeded (Admin, Member) + 2 added
        roles.Should().Contain(r => r.Name == "Role1");
        roles.Should().Contain(r => r.Name == "Role2");
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
    {
        var role = Role.Create("ExistRole", null);
        await _sut.AddAsync(role);
        await _context.SaveChangesAsync();

        var exists = await _sut.ExistsAsync(role.Id);

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
    {
        var exists = await _sut.ExistsAsync(Guid.NewGuid());

        exists.Should().BeFalse();
    }
}
