using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.RemoveRole
{
    public sealed class RemoveRoleEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/users/{userId:guid}/roles/{roleId:guid}", async (Guid userId, Guid roleId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RemoveRoleCommand(userId, roleId), ct);

                return result.IsSuccess ? Results.NoContent() : result.Error!.ToProblem("Remove role failed.");
            })
                .WithName("RemoveRole")
                .WithTags("Users")
                .WithSummary("Removes a role from a user")
                .WithDescription("Removes a specific role from a user by user ID and role ID")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}