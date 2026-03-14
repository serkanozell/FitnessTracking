using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.Login
{
    public sealed class LoginEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/users/login", async (LoginRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new LoginCommand(request.Email, request.Password),
                    ct);

                if (!result.IsSuccess)
                {
                    return result.Error!.ToProblem("Login failed.");
                }

                return Results.Ok(result.Data);
            })
                .WithName("Login")
                .WithTags("Users")
                .WithSummary("Authenticates a user")
                .WithDescription("Authenticates a user with email and password, returns a JWT token")
                .Accepts<LoginRequest>("application/json")
                .Produces<LoginResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .AllowAnonymous();
        }

        public sealed record LoginRequest(string Email, string Password);
    }
}
