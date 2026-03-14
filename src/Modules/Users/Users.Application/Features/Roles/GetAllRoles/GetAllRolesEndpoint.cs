using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Dtos;

namespace Users.Application.Features.Roles.GetAllRoles
{
    public sealed class GetAllRolesEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/roles", async (ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetAllRolesQuery(), ct);

                return Results.Ok(result.Data);
            })
                .WithName("GetAllRoles")
                .WithTags("Roles")
                .WithSummary("Gets all roles")
                .WithDescription("Returns a list of all available roles")
                .Produces<IReadOnlyList<RoleDto>>(StatusCodes.Status200OK);
        }
    }
}
