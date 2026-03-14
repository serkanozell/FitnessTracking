using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.ActivateUser
{
    public sealed class ActivateUserEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/users/{userId:guid}/activate", async (Guid userId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new ActivateUserCommand(userId), ct);

                if (!result.IsSuccess)
                {
                    return result.Error!.ToProblem("Activate user failed.");
                }

                return Results.NoContent();
            })
                .WithName("ActivateUser")
                .WithTags("Users")
                .WithSummary("Activates a user")
                .WithDescription("Reactivates a previously deleted or deactivated user")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
