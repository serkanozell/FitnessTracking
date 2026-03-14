using BuildingBlocks.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Dtos;

namespace Users.Application.Features.Users.GetProfile
{
    public sealed class GetProfileEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/users/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetProfileQuery(id), ct);

                if (!result.IsSuccess)
                {
                    return result.Error!.ToProblem("User not found.");
                }

                return Results.Ok(result.Data);
            })
                .WithName("GetProfile")
                .WithTags("Users")
                .WithSummary("Gets a user profile by ID")
                .WithDescription("Returns the user profile with role information")
                .Produces<UserDto>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
