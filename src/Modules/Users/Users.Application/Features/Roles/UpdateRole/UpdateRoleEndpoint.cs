using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Roles.UpdateRole
{
    public sealed class UpdateRoleEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/roles/{roleId:guid}", async (Guid roleId, UpdateRoleRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateRoleCommand(roleId, request.Name, request.Description), ct);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: "Update role failed.",
                                           detail: result.Error?.Message,
                                           statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.NoContent();
            })
                .WithName("UpdateRole")
                .WithTags("Roles")
                .WithSummary("Updates a role")
                .WithDescription("Updates the name and description of a role")
                .Accepts<UpdateRoleRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record UpdateRoleRequest(string Name, string? Description);
    }
}
