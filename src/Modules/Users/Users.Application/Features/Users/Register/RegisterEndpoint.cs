using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.Register
{
    public sealed class RegisterEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/users/register", async (RegisterRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new RegisterCommand(request.Email, request.Password, request.FirstName, request.LastName),
                    ct);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: "Registration failed.",
                                           detail: result.Error?.Message,
                                           statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.Created($"/api/v1/users/{result.Data}", new RegisterResponse(result.Data));
            })
                .WithName("Register")
                .WithTags("Users")
                .WithSummary("Registers a new user")
                .WithDescription("Creates a new user account with email and password")
                .Accepts<RegisterRequest>("application/json")
                .Produces<RegisterResponse>(StatusCodes.Status201Created)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);

        public sealed record RegisterResponse(Guid Id);
    }
}
