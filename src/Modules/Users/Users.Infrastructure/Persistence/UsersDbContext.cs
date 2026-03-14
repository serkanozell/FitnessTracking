using BuildingBlocks.Infrastructure.Persistence;
using Users.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace Users.Infrastructure.Persistence
{
    public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options)
        : ModuleDbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
