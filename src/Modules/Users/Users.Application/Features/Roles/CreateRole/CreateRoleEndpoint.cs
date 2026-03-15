using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Roles.CreateRole
{
    public sealed class CreateRoleEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/roles", async (CreateRoleRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new CreateRoleCommand(request.Name, request.Description),
                    ct);

                return result.IsSuccess ? Results.Created($"/api/v1/roles/{result.Data}", new CreateRoleResponse(result.Data)) :
                     result.Error!.ToProblem("Create role failed.");
            })
                .WithName("CreateRole")
                .WithTags("Roles")
                .WithSummary("Creates a new role")
                .WithDescription("Creates a new role with name and optional description")
                .Accepts<CreateRoleRequest>("application/json")
                .Produces<CreateRoleResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization("Admin");
        }

        public sealed record CreateRoleRequest(string Name, string? Description);

        public sealed record CreateRoleResponse(Guid Id);
    }
}