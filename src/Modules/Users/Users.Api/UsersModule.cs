using BuildingBlocks.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Users.Api
{
    public sealed class UsersModule : IModule
    {
        public Assembly ApplicationAssembly => typeof(Application.AssemblyReference).Assembly;

        public void Register(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapEndpointsFromAssembly(ApplicationAssembly);
        }
    }
}
