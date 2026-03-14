using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.AssignRole
{
    public sealed class AssignRoleEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/users/{userId:guid}/roles", async (Guid userId, AssignRoleRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new AssignRoleCommand(userId, request.RoleId), ct);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: "Assign role failed.",
                                           detail: result.Error?.Message,
                                           statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.NoContent();
            })
                .WithName("AssignRole")
                .WithTags("Users")
                .WithSummary("Assigns a role to a user")
                .WithDescription("Assigns a specific role to a user by user ID and role ID")
                .Accepts<AssignRoleRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record AssignRoleRequest(Guid RoleId);
    }
}
