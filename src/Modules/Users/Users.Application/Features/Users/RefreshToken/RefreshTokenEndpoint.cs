using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.RefreshToken
{
    public sealed class RefreshTokenEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/users/refresh-token", async (RefreshTokenRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new RefreshTokenCommand(request.RefreshToken), ct);

                if (!result.IsSuccess)
                {
                    return result.Error!.ToProblem("Token refresh failed.");
                }

                return Results.Ok(result.Data);
            })
                .WithName("RefreshToken")
                .WithTags("Users")
                .WithSummary("Refreshes an access token")
                .WithDescription("Exchanges a valid refresh token for a new access token and refresh token pair")
                .Accepts<RefreshTokenRequest>("application/json")
                .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .AllowAnonymous();
        }

        public sealed record RefreshTokenRequest(string RefreshToken);
    }
}
