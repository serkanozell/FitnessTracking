using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.ChangePassword
{
    public sealed class ChangePasswordEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/users/{userId:guid}/password", async (Guid userId, ChangePasswordRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword), ct);

                if (!result.IsSuccess)
                {
                    return result.Error!.ToProblem("Change password failed.");
                }

                return Results.NoContent();
            })
                .WithName("ChangePassword")
                .WithTags("Users")
                .WithSummary("Changes user password")
                .WithDescription("Changes the password of a user after verifying the current password")
                .Accepts<ChangePasswordRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    }
}
