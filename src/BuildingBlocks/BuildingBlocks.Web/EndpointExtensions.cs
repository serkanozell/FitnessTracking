using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace BuildingBlocks.Web
{
    public static class EndpointExtensions
    {
        public static void MapEndpointsFromAssembly(
            this IEndpointRouteBuilder app,
            Assembly assembly)
        {
            var endpoints = assembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsInterface &&
                    typeof(IEndpoint).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<IEndpoint>();

            foreach (var endpoint in endpoints)
            {
                endpoint.Map(app);
            }
        }
    }
}