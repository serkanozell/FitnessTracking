using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.DeleteUser
{
    public sealed class DeleteUserEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapDelete("/users/{userId:guid}", async (Guid userId, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new DeleteUserCommand(userId), ct);

                if (!result.IsSuccess)
                {
                    return Results.Problem(title: "Delete user failed.",
                                           detail: result.Error?.Message,
                                           statusCode: StatusCodes.Status400BadRequest);
                }

                return Results.NoContent();
            })
                .WithName("DeleteUser")
                .WithTags("Users")
                .WithSummary("Deletes a user")
                .WithDescription("Soft deletes a user by marking as deleted and inactive")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
