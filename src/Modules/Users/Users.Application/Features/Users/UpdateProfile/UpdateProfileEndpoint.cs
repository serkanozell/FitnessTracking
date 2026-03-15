using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Users.Application.Features.Users.UpdateProfile
{
    public sealed class UpdateProfileEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPut("/users/{userId:guid}/profile", async (Guid userId, UpdateProfileRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new UpdateProfileCommand(userId, request.FirstName, request.LastName), ct);

                return result.IsSuccess ? Results.NoContent() :
                     result.Error!.ToProblem("Update profile failed.");
            })
                .WithName("UpdateProfile")
                .WithTags("Users")
                .WithSummary("Updates user profile")
                .WithDescription("Updates the first name and last name of a user")
                .Accepts<UpdateProfileRequest>("application/json")
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        public sealed record UpdateProfileRequest(string FirstName, string LastName);
    }
}