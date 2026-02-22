using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web
{
    public interface IModule
    {
        Assembly ApplicationAssembly { get; }
        void Register(IServiceCollection services, IConfiguration configuration);
        void MapEndpoints(IEndpointRouteBuilder app);
    }
}