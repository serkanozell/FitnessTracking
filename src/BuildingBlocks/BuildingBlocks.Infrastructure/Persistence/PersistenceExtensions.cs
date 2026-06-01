using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public static class PersistenceExtensions
    {
        public const string DefaultConnectionStringName = "FitnessDbConnection";

        /// <summary>
        /// Bir modül DbContext'ini standart kurallarla kaydeder:
        /// ortak connection string, SaveChanges interceptor'ları ve
        /// modül şemasına izole edilmiş <c>__EFMigrationsHistory</c> tablosu.
        /// </summary>
        public static IServiceCollection AddModuleDbContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string schema,
            string? connectionStringName = null)
            where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(
                connectionStringName ?? DefaultConnectionStringName);

            services.AddDbContext<TContext>((sp, options) =>
            {
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString, sql =>
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", schema));
            });

            return services;
        }
    }
}
