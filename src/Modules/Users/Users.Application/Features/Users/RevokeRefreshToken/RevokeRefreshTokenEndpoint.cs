using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.RevokeRefreshToken
{
    public sealed class RevokeRefreshTokenEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/users/revoke-token", async (RevokeTokenRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RevokeRefreshTokenCommand(request.RefreshToken), ct);

                if (!result.IsSuccess)
                {
                    return result.Error!.ToProblem("Revoke failed.");
                }

                return Results.NoContent();
            })
                .WithName("RevokeRefreshToken")
                .WithTags("Users")
                .WithSummary("Revokes a refresh token")
                .WithDescription("Revokes the specified refresh token, effectively logging the user out")
                .Accepts<RevokeTokenRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record RevokeTokenRequest(string RefreshToken);
    }
}
