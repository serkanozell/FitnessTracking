using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.Web
{
    public interface IEndpoint
    {
        void Map(IEndpointRouteBuilder app);
    }
}