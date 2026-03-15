using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.CreateUser
{
    public sealed class CreateUserEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/users", async (CreateUserRequest request, ISender sender, CancellationToken ct) =>
            {
                var command = new CreateUserCommand(
                    request.Email,
                    request.Password,
                    request.FirstName,
                    request.LastName,
                    request.RoleIds ?? []);

                var result = await sender.Send(command, ct);

                return result.IsSuccess ?
                       Results.Created($"/api/v1/users/{result.Data}", new CreateUserResponse(result.Data))
                       : result.Error!.ToProblem("Create user failed.");
            })
                .WithName("CreateUser")
                .WithTags("Users")
                .WithSummary("Creates a new user with roles (Admin only)")
                .WithDescription("Admin creates a new user account with specified roles. If no roles provided, Member role is assigned by default.")
                .Accepts<CreateUserRequest>("application/json")
                .Produces<CreateUserResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .RequireAuthorization("Admin");
        }

        public sealed record CreateUserRequest(string Email, string Password, string FirstName, string LastName, List<Guid>? RoleIds);

        public sealed record CreateUserResponse(Guid Id);
    }
}