using BuildingBlocks.Domain.Security;
using BuildingBlocks.Infrastructure.Persistence;
using Users.Domain.Repositories;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Users.Infrastructure
{
    public static class UsersModuleExtensions
    {
        public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddModuleDbContext<UsersDbContext>(configuration, UsersSchema.Name);

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

            // Security
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}
